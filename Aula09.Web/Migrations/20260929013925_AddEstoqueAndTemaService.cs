using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aula09.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEstoqueAndTemaService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estoque",
                schema: "aula08",
                table: "Produtos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estoque",
                schema: "aula08",
                table: "Produtos");
        }
    }
}
