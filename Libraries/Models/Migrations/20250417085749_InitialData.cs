using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class InitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO [Product] (Name, Intro)
                VALUES (N'產品1', N'產品1介紹'), (N'產品2', N'產品2介紹')   

                INSERT INTO [ProductImage] (ProductId, Url)
                VALUES (1, 'http://example.com/product1.jpg'), (1, 'http://example.com/product1_2.jpg'), (2, 'http://example.com/product2.jpg')

                INSERT INTO [ProductSpec] (ProductId, Name, Description)
                VALUES (1, N'規格1', N'規格1描述'), (1, N'規格2', N'規格2描述'), (2, N'規格1', N'規格1描述')

                INSERT INTO News (Title, Summary)
                VALUES (N'新聞1', N'新聞1摘要'), (N'新聞2', N'新聞2摘要'), (N'新聞3', N'新聞3摘要')

                INSERT INTO [ProductNewsRelation] (ProductId, NewsId)
                VALUES (1, 1), (1, 2), (2, 3)

                INSERT INTO [Menu] (Name)
                VALUES (N'首頁'), (N'會員管理'), (N'權限管理'), (N'菜單管理'), (N'測試') 

                INSERT INTO [Permission] (Name)
                VALUES ('Admin'), ('User')

                INSERT INTO [PermissionMenuRelation] (PermissionId, MenuId, AllowCreate, AllowRead, AllowUpdate, AllowDelete)
                VALUES (1, 1, 1, 0, 1, 0), (1, 2, 1, 0, 0, 1), (1, 3, 1, 1, 0, 0), (1, 4, 0, 1, 1, 0), (2, 5, 0, 1, 0, 0)

                INSERT INTO [Member] (Name, Mobile, Email, CreateTime, Creator, UpdateTime, Updater)
                VALUES (N'test', '0912345678', 'test@mail.com', 1744860239, 0, 1744860239, 0)

                INSERT INTO [MemberPermission] (MemberId, PermissionId)
                VALUES (1, 1), (1, 2)

                INSERT INTO [SEO] (Name, Ext, LocalExt, LocalMirrorFile)
                VALUES ('1A', '.jpg', '', 1), ('2B', '', '.png', 0), ('3B', '.jpg', '.png', 0), ('4A', '.jpg', '.gif', 0), ('5C', '', '', 1)
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [ProductNewsRelation]
                DELETE FROM [ProductSpec]
                DELETE FROM [ProductImage]
                DELETE FROM [Product]
                DELETE FROM [News]
                DELETE FROM [PermissionMenuRelation]
                DELETE FROM [MemberPermission]
                DELETE FROM [Member]
                DELETE FROM [Permission]
                DELETE FROM [Menu]
                DELETE FROM [SEO]
            ");
        }
    }
}
