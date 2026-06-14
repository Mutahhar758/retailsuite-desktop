IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SupplyOrderMaster]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SupplyOrderMaster](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Title] [nvarchar](150) NULL,
        [Status] [int] NULL,
        CONSTRAINT [PK_SupplyOrderMaster] PRIMARY KEY CLUSTERED
        (
            [Id] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SupplyOrderDetail]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SupplyOrderDetail](
        [fkSupplyOrderId] [int] NULL,
        [fkCustomerId] [varchar](50) NULL,
        [SortOrder] [int] NULL,
        [Status] [int] NULL
    ) ON [PRIMARY]
END
GO
