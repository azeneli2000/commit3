using ConfiguratorWeb.App.Models.DiaryWeb;
using Digistat.FrameworkStd.Model.DiaryWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class DiaryWebViewModelBuilder
   {
      public static Models.DiaryWeb.Tag ToModel(this DiaryTag objSrc)
      {
         Models.DiaryWeb.Tag objRet = null;

         if (objSrc != null)
         {
            objRet = new Models.DiaryWeb.Tag()
            {
               IDtag = objSrc.DtgId,
               TextTag = objSrc.DtgName,
               ColorTag = objSrc.DtgColor,
               IndexTag = objSrc.DtgIndex,
               IsActiveTag = objSrc.DtgIsActive,
               IsSystemTag = objSrc.DtgIsSystem
            };
         }

         return objRet;
      }

      public static List<Tag> BuildTagsModelList(IEnumerable<DiaryTag> source)
      {
         try
         {
            return source.Select(x => x.ToModel()).ToList();
         }
         catch (Exception)
         {

            throw;
         }
      }

      public static Category ToModel(this DiaryCategory objSrc)
      {
         Category objRet = null;
         if (objSrc != null)
         {
            objRet = new Category()
            {
               ID = objSrc.DcaId,
               Text = objSrc.DcaName,
               Color = objSrc.DcaColor,
               LocationID = objSrc.DcaLocationRef.HasValue ? objSrc.DcaLocationRef.Value : 0,
               LocationName = objSrc.LocationDescription,
               Subjects = objSrc.DiarySubjects != null ? BuildSubjectsModelList(objSrc.DiarySubjects) : null,
               Phrases = objSrc.DiaryCategoryPhrases != null ? BuildCategoryPhrasesModelList(objSrc.DiaryCategoryPhrases) : null,
               Index = objSrc.DcaIndex,
               IsActive = objSrc.DcaIsActive,
               IsSystem = objSrc.DcaIsSystem
            };
         }
         return objRet;
      }


        public static SubjectPhrase ToModel(this DiarySubjectPhrase objSrc)
        {
            SubjectPhrase objRet = null;
            if (objSrc != null)
            {
                objRet = new SubjectPhrase()
                {
                    ID = objSrc.DspId,
                    Text = objSrc.DspText,
                    Index = objSrc.DspIndex,
                    Subject = objSrc.DspSubjectRef

                   
                };
            }
            return objRet;
        }
        public static List<Category> BuildCategoriesModelList(IEnumerable<DiaryCategory> source)
      {
         try
         {
            return source.Select(x => x.ToModel()).ToList();
         }
         catch (Exception)
         {

            throw;
         }
      }

      public static DiaryCategory ToEntity(this Category obj)
      {
         DiaryCategory ret = null;
         if (obj != null)
         {
            ret = new DiaryCategory()
            {
               DcaId = obj.ID,
               DcaName = obj.Text,
               DcaColor = obj.Color,
               DcaLocationRef = obj.LocationID > 0 ? obj.LocationID : null,
               DiaryCategoryPhrases = BuildPhrasesEntityList(obj.Phrases),
               DiarySubjects = BuildSubjectEntityList(obj.Subjects),
               DcaIndex = obj.Index,
               DcaIsActive = obj.IsActive,
               DcaIsSystem = obj.IsSystem
            };
         }
         return ret;
      }

      public static List<DiaryCategory> BuildCategoryEntityList(List<Category> source)
      {
         if (source != null)
         {
            return source.Select(x => x.ToEntity()).ToList();
         }
         return null;
      }


      public static Subject ToModel(this DiarySubject objSrc)
      {
         Subject ret = null;
         if (objSrc != null)
         {
            ret = new Subject()
            {
               ID = objSrc.DsbId,
               Category = objSrc.DsbCategoryRef,
               Text = objSrc.DsbName,
               Index = objSrc.DsbIndex,
               IsActive = objSrc.DsbIsActive,
               isEditable = objSrc.DsbIsEditable,
                SubjectsPhrases = objSrc.DiarySubjectPhrases != null ? BuildSubjectsModelListPH(objSrc.DiarySubjectPhrases) : null,

            };
         }
         return ret;
      }

      public static DiarySubject ToEntity(this Subject obj)
      {
         DiarySubject ret = null;
         if (obj != null)
         {
            ret = new DiarySubject()
            {
               DsbId = obj.ID,
               DsbName = obj.Text,
               DsbCategoryRef = obj.Category,
               DsbIndex = obj.Index,
               DsbIsActive = obj.IsActive,
               DiarySubjectPhrases = BuildSubjectEntityListPhrase(obj.SubjectsPhrases)


            };
         }
         return ret;
      }

        public static DiarySubjectPhrase ToEntity(this SubjectPhrase obj)
        {
            DiarySubjectPhrase ret = null;
            if (obj != null)
            {
                ret = new DiarySubjectPhrase()
                {
                   DspId = obj.ID,
                   DspIndex = obj.Index,
                   DspSubjectRef = obj.Subject,
                   DspText =  obj.Text


                };
            }
            return ret;
        }

        public static List<DiarySubject> BuildSubjectEntityList(IEnumerable<Subject> source)
      {
         if (source != null)
         {
            return source.Select(x => x.ToEntity()).ToList();
         }
         return null;
      }

        public static List<DiarySubjectPhrase> BuildSubjectEntityListPhrase(IEnumerable<SubjectPhrase> source)
        {
            if (source != null)
            {
                return source.Select(x => x.ToEntity()).ToList();
            }
            return null;
        }
        public static List<Subject> BuildSubjectsModelList(IEnumerable<DiarySubject> source)
      {
         try
         {
            return source.Select(x => x.ToModel()).ToList();
         }
         catch (Exception)
         {

            throw;
         }
      }

        public static List<SubjectPhrase> BuildSubjectsModelListPH(IEnumerable<DiarySubjectPhrase> source)
        {
            try
            {
                return source.Select(x => x.ToModel()).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static CategoryPhrases ToModel(this DiaryCategoryPhrase obj)
      {
         CategoryPhrases ret = null;
         if (obj != null)
         {
            ret = new CategoryPhrases()
            {
               ID = obj.DcpId,
               Category = obj.DcpCategoryRef,
               Text = obj.DcpText,
               Index = obj.DcpIndex
            };
         }
         return ret;
      }
      public static List<CategoryPhrases> BuildCategoryPhrasesModelList(IEnumerable<DiaryCategoryPhrase> source )
      {
         try
         {
            return source.Select(x => x.ToModel()).ToList();
         }
         catch (Exception)
         {

            throw;
         }
      }

      public static DiaryCategoryPhrase ToEntity(this CategoryPhrases obj)
      {
         DiaryCategoryPhrase ret = null;
         if (obj != null)
         {
            ret = new DiaryCategoryPhrase()
            {
               DcpId = obj.ID,
               DcpText = obj.Text,
               DcpCategoryRef = obj.Category,
               DcpIndex = obj.Index
               
            };
         }
         return ret;
      }

      public static List<DiaryCategoryPhrase> BuildPhrasesEntityList(IEnumerable<CategoryPhrases> source)
      {
         if (source != null)
         {
            return source.Select(x => x.ToEntity()).ToList();
         }
         return null;
      }
   }
}
