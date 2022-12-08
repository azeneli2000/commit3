using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.DiaryWeb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.BL
{
   public class DiaryWebManager : IDiaryWebManager
   {
      private DigistatDBContext mobjContext;
      private ILoggerService mobjLoggerService;
      public DiaryWebManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjContext = context;
         mobjLoggerService = loggerService;
      }

      public async Task<IEnumerable<DiaryTag>> GetTags()
      {
         try
         {
            return await mobjContext.Set<DiaryTag>().ToListAsync();
         }
         catch (Exception)
         {
            throw;
         }
      }

      public async Task<DiaryTag> GetTag(int id)
      {
         try
         {
            return await mobjContext.Set<DiaryTag>().Where(t => t.DtgId == id).FirstOrDefaultAsync();
         }
         catch (Exception)
         {
            throw;
         }
      }

      public async Task<DiaryTag> EditTag(DiaryTag tag)
      {
         try
         {
            if (tag == null)
               return null;

            if (tag.DtgId > 0)
            {
                    tag.Status = 1;
               mobjContext.Set<DiaryTag>().Update(tag);
            }
            else
            {
                    tag.Status = 0;
                    mobjContext.Set<DiaryTag>().Add(tag);
            }
            await mobjContext.SaveChangesAsync();
            return tag;
         }
         catch (Exception)
         {
            throw;
         }
      }

      public async Task<int> RemoveTag(int tagId)
      {
         var ret = 0;
         if (tagId <= 0)
            throw new Exception("Invalid Subject ID");

         var tags = mobjContext.Set<DiaryTag>();
         var item = tags.Where(s => s.DtgId == tagId).FirstOrDefault();
         if (item != null)
         {
            int count = tags.FromSqlRaw(@"SELECT TOP 1 DiaryTag.*  FROM [DiaryTagNoteLink] 
                                       inner join DiaryTag on dtg_ID =  dtl_TagID  
                                       where dtl_TagID = {0}", tagId).Count();

            if (count == 0) // the tag is not referenced by any Notes so it can be removed
            {
               tags.Remove(item);
               var res = await mobjContext.SaveChangesAsync();
               if (res == 1)
               {
                  ret = tagId;
               }
            }
         }
         return ret;
      }


      public async Task<IEnumerable<DiaryCategory>> GetCategories()
      {
         try
         {
            var categories = mobjContext.Set<DiaryCategory>();
            var locations = mobjContext.Set<Location>();
            var ret = (from c in categories
                       join l in locations on c.DcaLocationRef equals l.Id into join1
                       from loc in join1.DefaultIfEmpty()
                       select new DiaryCategory() { DcaId = c.DcaId, DcaColor = c.DcaColor, DcaLocationRef = c.DcaLocationRef, DcaName = c.DcaName, LocationDescription = loc != null ? loc.LocationName : string.Empty, DcaIndex = c.DcaIndex , DcaIsActive = c.DcaIsActive,DcaIsSystem= c.DcaIsSystem});

            return await ret.ToListAsync();
         }
         catch (Exception)
         {
            throw;
         }
      }

      public async Task<DiaryCategory> GetCategory(int id)
      {
         try
         {
                var categories = mobjContext.Set<DiaryCategory>().Include(a => a.DiaryCategoryPhrases).Include(a => a.DiarySubjects).Include("DiarySubjects.DiarySubjectPhrases");
   
           

            var locations = mobjContext.Set<Location>();
            var ret = (from c in categories.Where(c => c.DcaId == id)
                       join l in locations on c.DcaLocationRef equals l.Id into join1
                       from loc in join1.DefaultIfEmpty()
                       select new DiaryCategory() { DcaId = c.DcaId, DcaColor = c.DcaColor, DcaLocationRef = c.DcaLocationRef, DcaName = c.DcaName, LocationDescription = loc != null ? loc.LocationName : string.Empty, DiarySubjects = c.DiarySubjects, DiaryCategoryPhrases = c.DiaryCategoryPhrases, DcaIsActive = c.DcaIsActive, DcaIsSystem = c.DcaIsSystem }).FirstOrDefaultAsync();

        
            
            return await ret;
         }
         catch (Exception)
         {
            throw;
         }
      }

      public async Task<int> SaveCategory(DiaryCategory category)
      {
         int ret = 0;
         if (category == null)
            return 2;
         if (category.DcaId > 0)
         {
            if (category.DiarySubjects == null)
            {
               category.DiarySubjects = new List<DiarySubject>();
            }
            if (category.DiaryCategoryPhrases == null)
            {
               category.DiaryCategoryPhrases = new List<DiaryCategoryPhrase>();
            }
            var categories = mobjContext.Set<DiaryCategory>();
            var diarySubjects = mobjContext.Set<DiarySubject>();
            var diarySubjectsPh = mobjContext.Set<DiarySubjectPhrase>();
            var diaryCategoryPhrases = mobjContext.Set<DiaryCategoryPhrase>();
            var subjectComparer = new DiarySubjectIdComparer();
            var phraseComprarer = new DiaryCategoryPhraseIdComparer();
            var old = categories.Include(a => a.DiarySubjects).Include("DiarySubjects.DiarySubjectPhrases").Include(a => a.DiaryCategoryPhrases).Where(c => c.DcaId == category.DcaId).FirstOrDefault();
            var subjectToAdd = category.DiarySubjects.Except(old.DiarySubjects, subjectComparer);
            var subjectToRemove = old.DiarySubjects.Except(category.DiarySubjects, subjectComparer);
            var subjectToUpdate = category.DiarySubjects.Except(old.DiarySubjects, new DiarySubjectDiffComparer());
            var phrasesToAdd = category.DiaryCategoryPhrases.Except(old.DiaryCategoryPhrases, phraseComprarer);
            var phrasesToRemove = old.DiaryCategoryPhrases.Except(category.DiaryCategoryPhrases, phraseComprarer);
            var phraseToUpdate = category.DiaryCategoryPhrases.Except(old.DiaryCategoryPhrases, new DiaryCategoryPhraseDiffComparer());
            bool same = old.Equals(category);
            try
            {
                 
               mobjContext.BeginTransaction();
               if (!same)
               {
                  old.DcaName = category.DcaName;
                  old.DcaLocationRef = category.DcaLocationRef;
                  old.DcaColor = category.DcaColor;
                  old.DcaIsActive = category.DcaIsActive;
                  old.DcaIsSystem = category.DcaIsSystem;
                  categories.Update(old);
                  mobjContext.SaveChanges();
                  ret = 1; 
               }
               
               foreach (var obj in subjectToUpdate)
               {

                  var entity = diarySubjects.Where(a => a.DsbId == obj.DsbId).FirstOrDefault();
                  if (entity != null)
                  {
                     entity.DsbName = obj.DsbName;
                     entity.DsbIndex = obj.DsbIndex;
                     entity.DsbIsActive = obj.DsbIsActive;
                     diarySubjects.Update(entity);
                     mobjContext.SaveChanges();
                            //   if (!namePhToComparere.Contains(l.DspText))
                            if (obj.DiarySubjectPhrases != null)
                            {

                                foreach (var l in obj.DiarySubjectPhrases)
                                {
                                    if (l.DspId == 0)
                                    {
                                        diarySubjectsPh.Add(l);
                                    }
                                    else
                                    {

                                        var entityPhS = diarySubjectsPh.Where(a => a.DspId == l.DspId).FirstOrDefault();
                                        entityPhS.DspText = l.DspText;
                                        entityPhS.DspIndex = l.DspIndex;
                                        diarySubjectsPh.Update(entityPhS);


                                    }

                                }

                                var oldEntities = diarySubjectsPh.ToList();

                                List<int> idOld = new List<int>();
                                List<int> idNew = new List<int>();
                                foreach (var x in obj.DiarySubjectPhrases)
                                {
                                    if(x.DspId != 0 )
                                    {
                                        idNew.Add(x.DspId);  
                                    }
                                }
                                foreach (var x in oldEntities)
                                {
                                    
                                    idOld.Add(x.DspId);
                                    
                                }
                                
                                foreach(var y in idOld)
                                {
                                    if(!idNew.Contains(y))
                                    {
                                        var entityPhDel = diarySubjectsPh.Where(a => a.DspId == y).FirstOrDefault();
                                        diarySubjectsPh.Remove(entityPhDel);
                                    }
                                }
                                
                              

                                mobjContext.SaveChanges();
                            }
                            else
                            {
                                var entityPh = diarySubjectsPh.Where(a => a.DspSubjectRef == obj.DsbId).ToList();
                                if (entityPh != null)
                                {
                                    foreach (var l in entityPh)
                                    {
                                        diarySubjectsPh.Remove(l);
                                    }
                                }
                                mobjContext.SaveChanges();
                            }
                        }
                        else
                        {
                                var entityPh = diarySubjectsPh.Where(a => a.DspSubjectRef == obj.DsbId).ToList();
                                if (entityPh != null)
                                {
                                    foreach (var l in entityPh)
                                    {
                                        diarySubjectsPh.Remove(l);
                                    }
                                }
                                mobjContext.SaveChanges();
                        }
                     }
                    

               foreach (var obj in subjectToRemove.ToArray())
               {
                    var entityPh = diarySubjectsPh.Where(a => a.DspSubjectRef == obj.DsbId).ToList();
                    if (entityPh != null)
                    {
                        foreach (var l in entityPh)
                        {
                            diarySubjectsPh.Remove(l);
                        }
                    }
                    if (await RemoveSubject(obj) != obj.DsbId)
                    {
                        throw new Exception($"Error removing Subject {obj.DsbId}");
                    }

                  //await RemoveSubject(obj);

               }

               if (subjectToAdd.Count() > 0)
               {
                await diarySubjects.AddRangeAsync(subjectToAdd);
                        
                }

               foreach (var obj in phraseToUpdate)
               {
                  var entity = diaryCategoryPhrases.Where(a => a.DcpId == obj.DcpId).FirstOrDefault();
                  if (entity != null)
                  {
                     entity.DcpText = obj.DcpText;
                     entity.DcpIndex = obj.DcpIndex;
                     diaryCategoryPhrases.Update(entity);
                  }
               }

               if (phrasesToRemove.Count() > 0)
               {
                  diaryCategoryPhrases.RemoveRange(phrasesToRemove);
               }
               if (phrasesToAdd.Count() > 0)
               {
                  await mobjContext.Set<DiaryCategoryPhrase>().AddRangeAsync(phrasesToAdd);
               }

               await mobjContext.SaveChangesAsync();




               mobjContext.CommitTransaction();
               ret = 1;
            }
            catch (Exception ex)
            {
               mobjContext.RollbackTransaction();
               throw;
            }
         }
         else
         {
            try
            {
               mobjContext.BeginTransaction();
               mobjContext.Set<DiaryCategory>().Add(category);
               await mobjContext.SaveChangesAsync();
               mobjContext.CommitTransaction();
               ret = 0;
            }
            catch (Exception ex)
            {
               mobjContext.RollbackTransaction();
               throw;
            }
         }
         return ret;
      }

      public async Task<int> RemoveCategory(int categoryId)
      {
         var ret = 0;
         if (categoryId <= 0)
            throw new Exception("Invalid Category ID");

         var categories = mobjContext.Set<DiaryCategory>();
         var item = categories.Where(s => s.DcaId == categoryId).FirstOrDefault();
         if (item != null)
         {
            int count = categories.FromSqlRaw(@"SELECT DiaryCategory.*  FROM [DiaryCategory] 
                                       inner join DiarySubject sbj on sbj.dsb_CategoryRef = dca_ID
                                       inner join dbo.DiaryNoteVersion ver on ver.dnv_SubjectRef = sbj.dsb_ID
                                       where dca_ID = {0}
                                       UNION 
                                       SELECT DiaryCategory.*  FROM [DiaryCategory] 
                                       inner join DiarySubject sbj on sbj.dsb_CategoryRef = dca_ID
                                       inner join dbo.DiarySubjectTemplate tpl on tpl.dst_SubjectRef = sbj.dsb_ID  
                                       where dca_ID = {0}", categoryId).Count();
      
            if (count == 0) // the category is not referenced by any Notes or template so it can be removed
            {
               ret = 1; 
               try
               {
                  mobjContext.BeginTransaction();
                  categories.Remove(item);

                  var phrases = mobjContext.Set<DiaryCategoryPhrase>();
                  phrases.RemoveRange(phrases.Where(p => p.DcpCategoryRef == categoryId));

                  var subjects = mobjContext.Set<DiarySubject>();
                  subjects.RemoveRange(subjects.Where(s => s.DsbCategoryRef == categoryId));

                  var res = await mobjContext.SaveChangesAsync();
                  if (res > 0)
                  {
                     ret = categoryId;
                  }
                  mobjContext.CommitTransaction();

                  string strRlpText = item.DcaName.Replace(' ', '_');
                  string strCatPermission = "DIARY.NEW." + strRlpText.ToUpper();

                 bool resultPermissionOp =  await DeleteCategoryPermission(strCatPermission);
                  if (!resultPermissionOp)
                  {
                     throw new Exception("Failed to remove Permissions");
                  }
               }
               catch (Exception ex)
               {
                  mobjContext.RollbackTransaction();
                  throw;
               }
            }
            else
            {
               ret = 2;

            }
         }
         else
            throw new Exception($"Category [{categoryId}] not found");
         return ret;
      }

      public async Task<int> AddSubject(int categoryId, string subjectText)
      {
         if (categoryId <= 0)
            throw new Exception("Invalid category ID");

         var subjects = mobjContext.Set<DiarySubject>();
         var check = subjects.Where(s => s.DsbCategoryRef == categoryId && s.DsbName == subjectText).FirstOrDefault();
         if (check != null)
         {
            throw new Exception("Subject text already present for this category");
         }

         var subj = new DiarySubject() { DsbCategoryRef = categoryId, DsbName = subjectText };
         subjects.Add(subj);
         await mobjContext.SaveChangesAsync();
         return subj.DsbId;
      }

      public async Task<int> UpdateSubject(int subjectId, string subjectText)
      {
         if (subjectId <= 0)
            throw new Exception("Invalid subject ID");

         var subjects = mobjContext.Set<DiarySubject>();
         var subject = subjects.Where(p => p.DsbId == subjectId).FirstOrDefault();
         if (subject == null)
         {
            throw new Exception("Subject not found");
         }
         subject.DsbName = subjectText;
         subjects.Update(subject);
         await mobjContext.SaveChangesAsync();
         return subject.DsbId;
      }

      public async Task<int> RemoveSubject(int subjectId)
      {
         if (subjectId <= 0)
            throw new Exception("Invalid Subject ID");
         var subjects = mobjContext.Set<DiarySubject>();
         var item = subjects.Where(s => s.DsbId == subjectId).FirstOrDefault();
         return await RemoveSubject(item);
      }
      public async Task<int> RemoveSubject(DiarySubject subject)
      {
         var ret = 0;

         var subjects = mobjContext.Set<DiarySubject>();
         if (subject != null)
         {
            var check = subjects.FromSqlRaw(@"
                              SELECT [dsb_ID]
                                       ,[dsb_CategoryRef]
                                       ,[dsb_Name]
                                       ,[dsb_Index],[dsb_IsActive]
                                       FROM [DiarySubject] sbj
                                       inner join dbo.DiaryNoteVersion ver on ver.dnv_SubjectRef = sbj.dsb_ID
                                       where dsb_ID = {0}
									   UNION
									   SELECT [dsb_ID]
                                       ,[dsb_CategoryRef]
                                       ,[dsb_Name]
                                       ,[dsb_Index],[dsb_IsActive]
                                       FROM [DiarySubject] sbj
                                       inner join dbo.DiarySubjectTemplate tpl on tpl.dst_SubjectRef = sbj.dsb_ID  
                                       where dsb_ID = {0}", subject.DsbId).FirstOrDefault();

            if (check == null) // the subject is not referenced by other database tables so it can be removed
            {
               subjects.Remove(subject);
               //var res = await mobjContext.SaveChangesAsync();
               //if (res == 1)
               //{
                  ret = subject.DsbId;
               //}
            }
         }
         return ret;
      }
      public async Task<int> AddCategoryPhrases(int categoryId, string phrasesText)
      {
         if (categoryId <= 0)
            throw new Exception("Invalid category ID");

         var phrases = mobjContext.Set<DiaryCategoryPhrase>();
         var check = phrases.Where(p => p.DcpCategoryRef == categoryId && p.DcpText == phrasesText).FirstOrDefault();
         if (check != null)
         {
            throw new Exception("Phrase already present for this category");
         }

         var phrase = new DiaryCategoryPhrase() { DcpCategoryRef = categoryId, DcpText = phrasesText };
         phrases.Add(phrase);
         await mobjContext.SaveChangesAsync();
         return phrase.DcpId;
      }

      public async Task<int> UpdateCategoryPhrases(int phraseId, string phrasesText)
      {
         if (phraseId <= 0)
            throw new Exception("Invalid phrase ID");

         var phrases = mobjContext.Set<DiaryCategoryPhrase>();
         var phrase = phrases.Where(p => p.DcpId == phraseId).FirstOrDefault();
         if (phrase == null)
         {
            throw new Exception("Phrase not found");
         }
         phrase.DcpText = phrasesText;
         phrases.Update(phrase);
         await mobjContext.SaveChangesAsync();
         return phrase.DcpId;
      }

      public async Task<int> RemoveCategoryPhrases(int phraseID)
      {
         var ret = 0;
         if (phraseID <= 0)
            throw new Exception("Invalid CategoryPhrase ID");

         var phrases = mobjContext.Set<DiaryCategoryPhrase>();
         var item = phrases.Where(p => p.DcpId == phraseID).FirstOrDefault();
         if (item != null)
         {
            phrases.Remove(item);
            var res = await mobjContext.SaveChangesAsync();
            if (res == 1)
            {
               ret = phraseID;
            }
         }

         return ret;
      }

      public async Task<bool> ReorderCagtegory(Dictionary<int, int> catR)
      {

         try
         {
            mobjContext.BeginTransaction();


            foreach (var cat in catR)
            {
               var entity = mobjContext.Set<DiaryCategory>().Where(y => y.DcaId == cat.Key).FirstOrDefault();
               entity.DcaIndex = cat.Value;
               mobjContext.Set<DiaryCategory>().Update(entity);

            }

           await mobjContext.SaveChangesAsync();
            mobjContext.CommitTransaction();
           return true;
         }
         catch (Exception ex)
         {
            mobjContext.RollbackTransaction();
            return false;
         }

      }


      public async Task<bool> ReorderTag(Dictionary<int, int> tagR)
      {

         try
         {
            mobjContext.BeginTransaction();


            foreach (var tag in tagR)
            {
               var entity = mobjContext.Set<DiaryTag>().Where(y => y.DtgId == tag.Key).FirstOrDefault();
               entity.DtgIndex = tag.Value;
               mobjContext.Set<DiaryTag>().Update(entity);

            }

            await mobjContext.SaveChangesAsync();
            mobjContext.CommitTransaction();
            return true;
         }
         catch (Exception ex)
         {
            mobjContext.RollbackTransaction();
            return false;
         }

      }
      public async Task<bool> ResetCategoryIndex()
      {
         try
         {
            mobjContext.BeginTransaction();

            var entity = mobjContext.Set<DiaryCategory>().ToList().OrderBy(x=>x.DcaIndex);
            int index = 0;
            foreach (var cat in entity)
            {
               
               cat.DcaIndex = index;
               mobjContext.Set<DiaryCategory>().Update(cat);
               index++;

            }

            await mobjContext.SaveChangesAsync();
            mobjContext.CommitTransaction();
            return true; 
         }
         catch (Exception ex)
         {
            mobjContext.RollbackTransaction();
            return false;
         }
      }

      public async Task<bool> UpdatePermissionCategory(string categoryName, string newcategoryName)
      {
         try
         {
            DbTransaction transaction = null; 
            DbConnection connection = mobjContext.Database.GetDbConnection();
            string strSQL2 = @"UPDATE RolePermissions SET rpe_PermissionName='" + newcategoryName + "' where rpe_PermissionName='" + categoryName + "'";
            string strSQL = @"UPDATE Permissions SET FunctionName='" + newcategoryName + "' where FunctionName='" + categoryName + "'";
            var connectionState = connection.State;
            if (connectionState != ConnectionState.Open) connection.Open();
            using (var cmd = connection.CreateCommand())
            {
               transaction = connection.BeginTransaction();
               try
               {

                  cmd.Connection = connection;
                  cmd.Transaction = transaction; 

                  cmd.CommandText = strSQL;
                  await cmd.ExecuteNonQueryAsync();

                  cmd.CommandText = strSQL2;
                  await cmd.ExecuteNonQueryAsync();

                  transaction.Commit();
                  return true;
               }
               catch (Exception e)
               {
                  transaction.Rollback();
                  return false;
               }
            }


         }
         catch (Exception e)
         {

            return false;
         }
      }

      private async Task<bool> DeleteCategoryPermission(string categoryName)
      {
         try
         {
            DbTransaction transaction = null;
            DbConnection connection = mobjContext.Database.GetDbConnection();
            string strSQL2 = @"DELETE FROM RolePermissions where rpe_PermissionName='" + categoryName + "'";
            string strSQL = @"DELETE FROM Permissions  where FunctionName='" + categoryName + "'";
            var connectionState = connection.State;
            if (connectionState != ConnectionState.Open) connection.Open();
            using (var cmd = connection.CreateCommand())
            {
               transaction = connection.BeginTransaction();
               try
               {

                  cmd.Connection = connection;
                  cmd.Transaction = transaction;

                  cmd.CommandText = strSQL;
                  await cmd.ExecuteNonQueryAsync();

                  cmd.CommandText = strSQL2;
                  await cmd.ExecuteNonQueryAsync();

                  transaction.Commit();
                  return true;
               }
               catch (Exception e)
               {
                  transaction.Rollback();
                  return false;
               }
            }


         }
         catch (Exception e)
         {

            return false;
         }
      }

      public DiaryCategory CheckEditableSubjects(DiaryCategory cat)
      {

        
         var subjects = mobjContext.Set<DiarySubject>();
         foreach (var sub in cat.DiarySubjects)
         {
            var check = subjects.FromSqlRaw(@"
                              SELECT TOP 1 [dsb_ID]
                                       ,[dsb_CategoryRef]
                                       ,[dsb_Name]
                                       ,[dsb_Index],[dsb_IsActive]
                                       FROM [DiarySubject] sbj
                                       inner join dbo.DiaryNoteVersion ver on ver.dnv_SubjectRef = sbj.dsb_ID
                                       where dsb_ID = {0}
									   UNION
									   SELECT TOP 1 [dsb_ID]
                                       ,[dsb_CategoryRef]
                                       ,[dsb_Name]
                                       ,[dsb_Index],[dsb_IsActive]
                                       FROM [DiarySubject] sbj
                                       inner join dbo.DiarySubjectTemplate tpl on tpl.dst_SubjectRef = sbj.dsb_ID  
                                       where dsb_ID = {0}", sub.DsbId).FirstOrDefault();
            if (check != null)
            {
               sub.DsbIsEditable = false;
            }
            else
            {
               sub.DsbIsEditable = true;
            }
         }

         return cat;

      }

      public async Task<int> DeactiveCategory(int ID)
      {
         try
         {
            int ret = 0; 
            var categories = mobjContext.Set<DiaryCategory>();
            var item = categories.Where(s => s.DcaId == ID).FirstOrDefault();
            mobjContext.BeginTransaction();
            if (item.DcaIsActive)
            {
               item.DcaIsActive = false;
               categories.Update(item);
               mobjContext.CommitTransaction();
               var res = await mobjContext.SaveChangesAsync();
               if (res > 0)
               {
                  ret = 1;
               }
            }
            else
            {
               return 2;
            }
            return ret; 

         }
         catch (Exception)
         {
            mobjContext.RollbackTransaction();
            return 0; 
         }

      }

        public DiarySubject GetSubject(int ID)
        {
            var subject = mobjContext.Set<DiarySubject>().Where(x => x.DsbId == ID).Include(a => a.DiarySubjectPhrases).FirstOrDefault();
            return subject;
        }

        public class DiarySubjectIdComparer : IEqualityComparer<DiarySubject>
      {
         public bool Equals(DiarySubject x, DiarySubject y)
         {
            return x.DsbId == y.DsbId;
         }

         public int GetHashCode(DiarySubject obj)
         {
            return (obj.DsbId.ToString() + obj.DsbCategoryRef.ToString() + obj.DsbName).GetHashCode();
         }
      }

      public class DiarySubjectDiffComparer : IEqualityComparer<DiarySubject>
      {
         public bool Equals(DiarySubject x, DiarySubject y)
         {
                bool result = true;
                if (x.DiarySubjectPhrases == null && y.DiarySubjectPhrases == null)
                    result = false;
                if (x.DiarySubjectPhrases != null && y.DiarySubjectPhrases == null)
                    result = false;
                if (x.DiarySubjectPhrases == null && y.DiarySubjectPhrases != null)
                    result = false;
                if (x.DiarySubjectPhrases != null && y.DiarySubjectPhrases != null)
                {
                    if (x.DiarySubjectPhrases.Count() == y.DiarySubjectPhrases.Count())
                    {
                        var xH = x.DiarySubjectPhrases.ToArray();
                        var yH = y.DiarySubjectPhrases.ToArray();
                        for (int i = 0; i < xH.Length; i++)
                        {
                            if (xH[i] != yH[i])
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                    else
                        result = false;
                }
                return x.DsbId == y.DsbId && x.DsbCategoryRef == y.DsbCategoryRef && x.DsbName == y.DsbName && x.DsbIsActive == y.DsbIsActive && x.DsbIndex == y.DsbIndex && result ;
         }
         public int GetHashCode(DiarySubject obj)
         {
            return (obj.DsbId.ToString() + obj.DsbCategoryRef.ToString() + obj.DsbName).GetHashCode();
         }
      }


      public class DiaryCategoryPhraseIdComparer : IEqualityComparer<DiaryCategoryPhrase>
      {
         public bool Equals(DiaryCategoryPhrase x, DiaryCategoryPhrase y)
         {
            return x.DcpId == y.DcpId && x.DcpIndex == y.DcpIndex;
         }

         public int GetHashCode(DiaryCategoryPhrase obj)
         {
            return (obj.DcpId.ToString() + obj.DcpCategoryRef.ToString() + obj.DcpText).GetHashCode();
         }
      }

      public class DiaryCategoryPhraseDiffComparer : IEqualityComparer<DiaryCategoryPhrase>
      {
         public bool Equals(DiaryCategoryPhrase x, DiaryCategoryPhrase y)
         {
            return x.DcpId == y.DcpId && x.DcpCategoryRef == y.DcpCategoryRef && x.DcpText == y.DcpText;
         }
         public int GetHashCode(DiaryCategoryPhrase obj)
         {
            return (obj.DcpId.ToString() + obj.DcpCategoryRef.ToString() + obj.DcpText).GetHashCode();
         }
      }
   }

}

