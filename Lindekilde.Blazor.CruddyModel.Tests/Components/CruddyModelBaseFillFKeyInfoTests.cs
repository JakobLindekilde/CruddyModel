using Lindekilde.Blazor.CruddyModel.Components;

namespace Lindekilde.Blazor.CruddyModel.Tests.Components;

public class CruddyModelBaseFillFKeyInfoTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void FillFKeyInfo_NullOrEmpty_ReturnsEmptyList(string? fKey)
    {
        var result = CruddyModelBase<object>.FillFKeyInfoList(fKey, "dbo", false);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("CategoryId: Categories, Id")]
    [InlineData("CategoryId: Categories, Id, Name, Extra")]
    [InlineData("CategoryId:")]
    public void FillFKeyInfo_WrongNumberOfValues_ReturnsEmptyList(string fKey)
    {
        var result = CruddyModelBase<object>.FillFKeyInfoList(fKey, "dbo", false);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void FillFKeyInfo_ValidInput_ReturnsPopulatedInfo()
    {
        var result = CruddyModelBase<object>.FillFKeyInfoList("CategoryId1: Categories1, Id1, Name1; CategoryId2: Categories2, Id2, Name2", "dbo", false);

        Assert.NotNull(result);
        
        Assert.Equal("CategoryId1", result[0].FKey);
        Assert.Equal("dbo.Categories1", result[0].PrimaryTable);
        Assert.Equal("Id1", result[0].PrimaryId);
        Assert.Equal("Name1", result[0].PrimaryDisplay);

        Assert.Equal("CategoryId2", result[1].FKey);
        Assert.Equal("dbo.Categories2", result[1].PrimaryTable);
        Assert.Equal("Id2", result[1].PrimaryId);
        Assert.Equal("Name2", result[1].PrimaryDisplay);
    }

    [Fact]
    public void FillFKeyInfo_ExtraWhitespace_TrimsValues()
    {
        var result = CruddyModelBase<object>.FillFKeyInfoList("  CategoryId  :  Categories ,  Id  ,  Name  ", "dbo", false);

        Assert.NotNull(result);
        Assert.Equal("CategoryId", result[0].FKey);
        Assert.Equal("dbo.Categories", result[0].PrimaryTable);
        Assert.Equal("Id", result[0].PrimaryId);
        Assert.Equal("Name", result[0].PrimaryDisplay);
    }

    [Fact]
    public void FillFKeyInfo_NoSpaces_ParsesCorrectly()
    {
        var result = CruddyModelBase<object>.FillFKeyInfoList("CategoryId:Categories,Id,Name", "dbo", false);

        Assert.NotNull(result);
        Assert.Equal("dbo.Categories", result[0].PrimaryTable);
    }


    [Theory]
    [InlineData("CategoryID")]
    [InlineData("CategoryId")]
    [InlineData("CategoriesId")]
    [InlineData("CategoriesID")]
    public void GuessFKeyInfo_ReturnsPopulatedInfo(string fKey)
    {
        var result = CruddyModelBase<object>.GuessFKeyInfo(fKey, "dbo", true);
        Assert.Equal("dbo.Categories", result?.PrimaryTable);
        Assert.Equal("Id", result?.PrimaryId);
        Assert.Equal("Name", result?.PrimaryDisplay);
    }

    [Theory]
    [InlineData("Category")]
    [InlineData("")]
    [InlineData("Identity")]
    public void GuessFKeyInfo_NotEndingWithId_ReturnsNull(string fKey)
    {
        var result = CruddyModelBase<object>.GuessFKeyInfo(fKey, "dbo", true);
        Assert.Null(result);
    }

    [Fact]
    public void GuessFKeyInfo_SetsFKey()
    {
        var result = CruddyModelBase<object>.GuessFKeyInfo("CategoryId", "dbo", true);
        Assert.Equal("CategoryId", result?.FKey);
    }

    [Fact]
    public void GuessFKeyInfo_NoPluralize_KeepsTableName()
    {
        var result = CruddyModelBase<object>.GuessFKeyInfo("CategoryId", "dbo", false);
        Assert.Equal("dbo.Category", result?.PrimaryTable);
    }

    [Fact]
    public void GuessFKeyInfo_AlreadyPluralized_NotPluralizedAgain()
    {
        var result = CruddyModelBase<object>.GuessFKeyInfo("CategoriesId", "dbo", true);
        Assert.Equal("dbo.Categories", result?.PrimaryTable);
    }

    [Fact]
    public void GuessFKeyInfo_TableWithSchema_DoesNotPrependDefaultSchema()
    {
        var result = CruddyModelBase<object>.GuessFKeyInfo("shop.CategoryId", "dbo", false);
        Assert.Equal("shop.Category", result?.PrimaryTable);
    }

    [Fact]
    public void GuessFKeyInfo_UsesDefaultSchema()
    {
        var result = CruddyModelBase<object>.GuessFKeyInfo("CategoryId", "sales", false);
        Assert.Equal("sales.Category", result?.PrimaryTable);
    }

}