CREATE TABLE [dbo].[ProductionStore]
(
    [Id] varchar(100) NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(255) NOT NULL,
    [Region] VARCHAR(255) NULL,
    [WIPURL] VARCHAR(MAX) NULL,
    [WIPAllocatedSize] DECIMAL(38,10) NULL,
    [ArchiveURL] VARCHAR(MAX) NULL,
    [ArchiveAllocatedSize] DECIMAL(38,10) NULL,
    [ScaleDownTime] DATETIME NULL,
    [ScaleUpTimeInterval] DATETIME NULL,
    [MinimumFreeSize] DECIMAL(38,10) NULL,
    [MinimumFreeSpace] DECIMAL(38,10) NULL,
    [OfflineTime] VARCHAR(50) NULL,
    [OnlineTime]  VARCHAR(50) NULL,
    [ProductionOfflineTimeInterval] DECIMAL(38, 10) NULL,
    [ManagerRoleGroupNames] VARCHAR(MAX) NULL,
    [UserRoleGroupNames] VARCHAR(MAX) NULL,
    [WIPKeyName] VARCHAR(50) NOT NULL default ('no-key-name'),
    [ArchiveKeyName] VARCHAR(50) NOT NULL default ('no-key-name')
)