CREATE TABLE [dbo].[Production]
(    
    [Id] varchar(100) NOT NULL PRIMARY KEY, 
    [Name] varchar(255) NOT NULL, 
    [ProductionStoreId] varchar(100) NOT NULL, 
    [WIPUrl] varchar(max),
    [ArchiveId] varchar(255),
    [ArchiveUrl] varchar(max),
    [Status] varchar(50),
    [CreatedDateTime] DATETIME NOT NULL DEFAULT getdate(), 
    [LastSyncDateTime] DATETIME,
    [SizeInBytes] BIGINT,
    [DeletedFlag] BIT,
    [DeletedDateTime] DATETIME,
    [ModifiedDateTime] DATETIME,
    [GetStatusQueryUri] varchar(max),
    [StateChangeDateTime] DATETIME,
    CONSTRAINT FK_ProdStoreId_ProdStoreDetail FOREIGN KEY (ProductionStoreId)
        REFERENCES [dbo].[ProductionStore] (Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
)