/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [IntProductId]
      ,[StrProductName]
      ,[NumUnitPrice]
      ,[NumStock]
  FROM [dbERP].[dbo].[products]