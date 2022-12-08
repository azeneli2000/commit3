using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ConfiguratorWeb.App.Models.ExportScheduler
{
   [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
   public partial class ExportJobView
   {
      public ExportJobView()
      {

      }

      public ExportJobView(ExportJobView job)
      {
         ID = job.ID;
         Host = job.Host;
         Name = job.Name;
         ReportMasterTemplate = job.ReportMasterTemplate;
         Status = job.Status;
         FileName = job.FileName;
         FileFormat = job.FileFormat;
         Timeout = job.Timeout;
         SaveOnFileSystem = job.SaveOnFileSystem;
         SendMail = job.SendMail;
         TriggerIsScheduled = job.TriggerIsScheduled;
         TriggerIsOnMessage = job.TriggerIsOnMessage;
         TriggerScheduledCron = job.TriggerScheduledCron;
         TriggerMessage = job.TriggerMessage;
         FileSystemRootPath = job.FileSystemRootPath;
         FileSystemSubFolder = job.FileSystemSubFolder;
         FileSystemUsername = job.FileSystemUsername;
         FileSystemPassword = job.FileSystemPassword;
         FileSystemDaysToLive = job.FileSystemDaysToLive;
         EmailFromName = job.EmailFromName;
         EmailFromAddress = job.EmailFromAddress;
         EmailToAddresses = job.EmailToAddresses;
         EmailSubject = job.EmailSubject;
         EmailBody = job.EmailBody;
         ParameterQuery = job.ParameterQuery;
         LastRunStatus = job.LastRunStatus;
         LastRunStatusCode = job.LastRunStatusCode;
         LastRunDateTime = job.LastRunDateTime;
         LastRunMessage = job.LastRunMessage;
         LastUpdate = job.LastUpdate;
         UserID = job.UserID;
      }

      public virtual int ID { get; set; } // int

      [MaxLength(100)]
      public virtual string Host { get; set; } // nvarchar(100)

      [MaxLength(250)]
      public virtual string Name { get; set; } // nvarchar(250)

      [MaxLength(100)]
      public virtual string ReportMasterTemplate { get; set; } // nvarchar(100)

      public virtual short? Status { get; set; } // smallint

      [MaxLength(200)]
      public virtual string FileName { get; set; } // nvarchar(200)

      public virtual short? FileFormat { get; set; } // smallint

      public virtual int? Timeout { get; set; } // int

      public virtual bool? SaveOnFileSystem { get; set; } // bit

      public virtual bool? SendMail { get; set; } // bit

      public virtual bool? TriggerIsScheduled { get; set; } // bit

      public virtual bool? TriggerIsOnMessage { get; set; } // bit

      [MaxLength(500)]
      public virtual string TriggerScheduledCron { get; set; } // nvarchar(500)

      [MaxLength(50)]
      public virtual string TriggerMessage { get; set; } // nvarchar(50)

      [MaxLength(300)]
      public virtual string FileSystemRootPath { get; set; } // nvarchar(300)

      [MaxLength(100)]
      public virtual string FileSystemSubFolder { get; set; } // nvarchar(100)

      [MaxLength(100)]
      public virtual string FileSystemUsername { get; set; } // nvarchar(100)

      [MaxLength(50)]
      public virtual string FileSystemPassword { get; set; } // nvarchar(50)

      public virtual int? FileSystemDaysToLive { get; set; } // int

      [MaxLength(150)]
      public virtual string EmailFromName { get; set; } // nvarchar(150)

      [MaxLength(150)]
      public virtual string EmailFromAddress { get; set; } // nvarchar(150)

      [MaxLength(500)]
      public virtual string EmailToAddresses { get; set; } // nvarchar(500)

      [MaxLength(100)]
      public virtual string EmailSubject { get; set; } // nvarchar(100)

      [MaxLength]
      public virtual string EmailBody { get; set; } // nvarchar(max)

      [MaxLength]
      public virtual string ParameterQuery { get; set; } // nvarchar(max)

      [MaxLength(50)]
      public virtual string LastRunStatus { get; set; } // nvarchar(50)
      public virtual int LastRunStatusCode { get; set; } // nvarchar(50)

      public virtual DateTime? LastRunDateTime { get; set; } // datetime

      [MaxLength]
      public virtual string LastRunMessage { get; set; } // nvarchar(max)

      public virtual DateTime? LastUpdate { get; set; } // datetime

      [MaxLength(50)]
      public virtual string UserID { get; set; } // nvarchar(50)

      public override bool Equals(object obj)
      {
         return obj is ExportJobView job &&
                ID == job.ID &&
                Host == job.Host &&
                Name == job.Name &&
                ReportMasterTemplate == job.ReportMasterTemplate &&
                Status == job.Status &&
                FileName == job.FileName &&
                FileFormat == job.FileFormat &&
                Timeout == job.Timeout &&
                SaveOnFileSystem == job.SaveOnFileSystem &&
                SendMail == job.SendMail &&
                TriggerIsScheduled == job.TriggerIsScheduled &&
                TriggerIsOnMessage == job.TriggerIsOnMessage &&
                TriggerScheduledCron == job.TriggerScheduledCron &&
                TriggerMessage == job.TriggerMessage &&
                FileSystemRootPath == job.FileSystemRootPath &&
                FileSystemSubFolder == job.FileSystemSubFolder &&
                FileSystemUsername == job.FileSystemUsername &&
                FileSystemPassword == job.FileSystemPassword &&
                FileSystemDaysToLive == job.FileSystemDaysToLive &&
                EmailFromName == job.EmailFromName &&
                EmailFromAddress == job.EmailFromAddress &&
                EmailToAddresses == job.EmailToAddresses &&
                EmailSubject == job.EmailSubject &&
                EmailBody == job.EmailBody &&
                ParameterQuery == job.ParameterQuery &&
                LastRunStatus == job.LastRunStatus &&
                LastRunStatusCode == job.LastRunStatusCode &&
                LastRunDateTime == job.LastRunDateTime &&
                LastRunMessage == job.LastRunMessage &&
                LastUpdate == job.LastUpdate &&
                UserID == job.UserID;
      }


      private string GetDebuggerDisplay()
      {
         return ToString();
      }


   }
}