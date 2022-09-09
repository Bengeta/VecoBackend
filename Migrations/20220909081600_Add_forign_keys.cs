using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VecoBackend.Migrations
{
    public partial class Add_forign_keys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_task_id",
                table: "UserTasks",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_user_id",
                table: "UserTasks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_TaskImage_imageId",
                table: "TaskImage",
                column: "imageId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskImage_UserTaskId",
                table: "TaskImage",
                column: "UserTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTokens_UserId",
                table: "NotificationTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageStorage_userId",
                table: "ImageStorage",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageStorage_Users_userId",
                table: "ImageStorage",
                column: "userId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTokens_Users_UserId",
                table: "NotificationTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskImage_ImageStorage_imageId",
                table: "TaskImage",
                column: "imageId",
                principalTable: "ImageStorage",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskImage_UserTasks_UserTaskId",
                table: "TaskImage",
                column: "UserTaskId",
                principalTable: "UserTasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageStorage_Users_userId",
                table: "ImageStorage");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationTokens_Users_UserId",
                table: "NotificationTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskImage_ImageStorage_imageId",
                table: "TaskImage");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskImage_UserTasks_UserTaskId",
                table: "TaskImage");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_Tasks_task_id",
                table: "UserTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_Users_user_id",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_task_id",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_user_id",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_TaskImage_imageId",
                table: "TaskImage");

            migrationBuilder.DropIndex(
                name: "IX_TaskImage_UserTaskId",
                table: "TaskImage");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTokens_UserId",
                table: "NotificationTokens");

            migrationBuilder.DropIndex(
                name: "IX_ImageStorage_userId",
                table: "ImageStorage");
        }
    }
}
