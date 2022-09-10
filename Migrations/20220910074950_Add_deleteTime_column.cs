using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VecoBackend.Migrations
{
    public partial class Add_deleteTime_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_Tasks_task_id",
                table: "UserTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_Users_user_id",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_user_id",
                table: "UserTasks");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserTasks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserTasks",
                newName: "taskStatus");

            migrationBuilder.RenameColumn(
                name: "task_status",
                table: "UserTasks",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "task_id",
                table: "UserTasks",
                newName: "TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTasks_task_id",
                table: "UserTasks",
                newName: "IX_UserTasks_TaskId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Tasks",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Tasks",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "points",
                table: "Tasks",
                newName: "Points");

            migrationBuilder.RenameColumn(
                name: "isSeen",
                table: "Tasks",
                newName: "IsSeen");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Tasks",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "deadline",
                table: "Tasks",
                newName: "Deadline");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Tasks",
                newName: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "UserTasks",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_UserId",
                table: "UserTasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_Tasks_TaskId",
                table: "UserTasks",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_Users_UserId",
                table: "UserTasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_Tasks_TaskId",
                table: "UserTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_Users_UserId",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_UserId",
                table: "UserTasks");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "UserTasks");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserTasks",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "taskStatus",
                table: "UserTasks",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserTasks",
                newName: "task_status");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "UserTasks",
                newName: "task_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserTasks_TaskId",
                table: "UserTasks",
                newName: "IX_UserTasks_task_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Tasks",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Tasks",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Points",
                table: "Tasks",
                newName: "points");

            migrationBuilder.RenameColumn(
                name: "IsSeen",
                table: "Tasks",
                newName: "isSeen");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Tasks",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Deadline",
                table: "Tasks",
                newName: "deadline");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tasks",
                newName: "id");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_user_id",
                table: "UserTasks",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_Tasks_task_id",
                table: "UserTasks",
                column: "task_id",
                principalTable: "Tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_Users_user_id",
                table: "UserTasks",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
