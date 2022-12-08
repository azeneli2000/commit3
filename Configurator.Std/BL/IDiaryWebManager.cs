using Digistat.FrameworkStd.Model.DiaryWeb;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.BL
{
   public interface IDiaryWebManager 
   {
      Task<IEnumerable<DiaryTag>> GetTags();
      Task<DiaryTag> GetTag(int id);
      Task<DiaryTag> EditTag(DiaryTag tag);
      Task<int> RemoveTag(int tagId);
      Task<IEnumerable<DiaryCategory>> GetCategories();
      Task<DiaryCategory> GetCategory(int id);
      Task<int> SaveCategory(DiaryCategory category);
      Task<int> RemoveCategory(int categoryId);
      Task<int> AddSubject(int categoryId, string subjectText);
      Task<int> UpdateSubject(int subjectId, string subjectText);
      Task<int> RemoveSubject(DiarySubject subject);
      Task<int> RemoveSubject(int subjectId);
      Task<int> AddCategoryPhrases(int categoryId, string phrasesText);
      Task<int> RemoveCategoryPhrases(int phraseId);
      Task<int> UpdateCategoryPhrases(int phraseId, string phrasesText);

      Task<bool> ReorderCagtegory(Dictionary<int,int>catR);
      Task<bool> ReorderTag(Dictionary<int,int>tagR);

      Task<bool> ResetCategoryIndex();

      Task<bool> UpdatePermissionCategory(string categoryName, string newcategoryName);

      Task<int> DeactiveCategory(int ID);
      DiarySubject GetSubject(int ID);
      DiaryCategory CheckEditableSubjects(DiaryCategory cat);

   }
}
