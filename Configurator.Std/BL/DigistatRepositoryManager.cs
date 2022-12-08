using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using Configurator.Std.BL.Hubs;
using Configurator.Std.Exceptions;
using Configurator.Std.Enums;
using Digistat.Dal.Interfaces;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Enums;
using Configurator.Std.BL.DasDrivers;
using Configurator.Std.Helpers;
using System.IO;
using Configurator.Std.BL.Configurator;
using System.IO.Compression;

namespace Configurator.Std.BL
{
    public class DigistatRepositoryManager : DalManagerBase<DigistatRepository>, IDigistatRepositoryManager
    {


        protected readonly IMessageCenterManager mobjMsgCtrMgr;
        protected readonly IConfiguratorWebConfiguration mobjWebCfg;
        #region Costructors

        public DigistatRepositoryManager(DigistatDBContext context, ILoggerService loggerService, IMessageCenterManager msgCtrSvc,
           IConfiguratorWebConfiguration confCfg)
        {
            mobjDbContext = context;
            mobjLoggerService = loggerService;
            mobjMsgCtrMgr = msgCtrSvc;
            mobjWebCfg = confCfg;
        }

        #endregion

        public DigistatRepository Create(string filename, byte[] image, bool isArchive)
        {
            DigistatRepository objRet = null;

            objRet = new DigistatRepository();
            string strExt = System.IO.Path.GetExtension(filename);
            if (isArchive)
            {
                objRet.Type = (short)Digistat.FrameworkStd.Enums.DigistatRepositoryType.ARCHIVE;
            }
            else
            {
                objRet.Type = (short)Digistat.FrameworkStd.Helpers.ConversionsHelper.GetRepositoryTypeFromExt(strExt);
            }

            objRet.ID = Guid.NewGuid().ToString();
            objRet.FileName = filename;
            objRet.Stream = image;
            objRet.Size = image.Length;
            objRet.LastUpdate = DateTime.Now;
            objRet.Application = string.Empty;

            mobjDbContext.Set<DigistatRepository>().Add(objRet);

            mobjDbContext.SaveChanges();
            return objRet;
        }


        public IQueryable<DigistatRepository> GetQueryable()
        {
            try
            {
                //List<Bed> objBedList = null;
                IQueryable<DigistatRepository> repository = mobjDbContext.Set<DigistatRepository>().Select(e => new DigistatRepository
                {
                    ID = e.ID,
                    Application = e.Application,
                    FileName = e.FileName,
                    LastUpdate = e.LastUpdate,
                    Size = e.Size,
                    Type = e.Type
                });
                return repository;
            }
            catch (Exception e)
            {
                string errMsg = "Error GetQueryable";
                mobjLoggerService.ErrorException(e, errMsg);
                throw new Exception(errMsg, e);
            }
        }



        public void Delete(string id)
        {
            try
            {
                var mobjRepoContext = mobjDbContext.Set<DigistatRepository>();
                mobjRepoContext.Remove(mobjRepoContext.Where(p => p.ID == id).FirstOrDefault());
                mobjDbContext.SaveChanges();

            }
            catch (Exception e)
            {
                string errMsg = "Error Delete DigistatRepository with ID {id}";
                mobjLoggerService.ErrorException(e, errMsg);
                throw new Exception(errMsg, e);
            }
        }



        public DigistatRepository Get(string id)
        {
            var mobjRepoContext = mobjDbContext.Set<DigistatRepository>();
            return mobjRepoContext.Where(p => p.ID == id).FirstOrDefault();
        }


        public byte[] CreateArchiveForDigistatRepository(List<CachedFile> objCacheFiles, string archiveName)
        {
            byte[] objRet = null;
            try
            {
                string strPath = CacheDigistatRepositoryFiles(objCacheFiles);
                objRet = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompression.CompressFolderContent(strPath, archiveName);
                DeleteDigistatRepositoryFolder(strPath);
            }
            catch (Exception e)
            {
                mobjLoggerService.ErrorException(e, "Error in CreateArchiveForDigistatRepository ");
                throw;
            }
            return objRet;
        }

        private void DeleteDigistatRepositoryFolder(string folder)
        {
            try
            {
                System.IO.Directory.Delete(folder, true);
            }
            catch (Exception e)
            {
                mobjLoggerService.ErrorException(e, "Error in DeleteDigistatRepositoryFolder ");
            }
        }


        public byte[] UncompressArchive(string archiveName, byte[] archiveContent)
        {
            byte[] objRet = null;
            string strCacheID = System.Guid.NewGuid().ToString();
            string strpath = CachingHelper.GetDigistatRepoPath(strCacheID, mobjWebCfg);
            if (Directory.Exists(strpath))
            {
                try
                {
                    Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompression.ExtractAllUsingFramework(archiveContent, strpath);
                    //Create a "normal" zip file for download
                    string strZipFileName = Path.Combine(strpath, archiveName + ".zip");
                    using (ZipArchive archive = ZipFile.Open(strZipFileName, ZipArchiveMode.Create))
                    {
                        foreach (string s in Directory.GetFiles(strpath))
                        {
                            if (!s.ToLower().Equals(strZipFileName.ToLower()))
                            {
                                archive.CreateEntryFromFile(s, Path.GetFileName(s));
                            }
                        }
                    }

                    //Read zipped file
                    objRet = File.ReadAllBytes(strZipFileName);

                    //Delete temporary folder
                    Directory.Delete(strpath, true);
                }
                catch (Exception e)
                {
                    mobjLoggerService.ErrorException(e, "Error on DigistatRepositoryManager.UncompressArchive");
                    throw;

                }
            }
            else
            {
                mobjLoggerService.Error($"DigistatRepositoryManager: Unable to uncompress archive, missing {strpath}");
            }
            return objRet;
        }


        private string CacheDigistatRepositoryFiles(List<CachedFile> objCacheFiles)
        {
            string strRet = null;
            string strCacheID = System.Guid.NewGuid().ToString();
            try
            {
                string strpath = CachingHelper.GetDigistatRepoPath(strCacheID, mobjWebCfg);
                if (Directory.Exists(strpath))
                {
                    foreach (CachedFile updFile in objCacheFiles)
                    {
                        string destination = Path.Combine(strpath, updFile.Name);
                        File.WriteAllBytes(destination, updFile.Content);
                    }

                }
                strRet = strpath;
            }
            catch
            {
                throw;
            }
            return strRet;
        }





    }
}
