<%@ Page Language="VB" AutoEventWireup="false" CodeFile="FileManager.aspx.vb" Inherits="FileManager" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <title>File Manager</title>
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <style>

        body {
            margin: 20px;
            background: #f5f6fa;
            font-family: Segoe UI, Arial;
        }

        .toolbar {
            margin-bottom: 20px;
        }

        .breadcrumb {
            font-size: 16px;
            font-weight: 600;
            color: #444;
            margin-top: 10px;
        }

        .search-box {
            width: 100%;
            max-width: 400px;
        }

        .search-box input {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid #ddd;
            border-radius: 6px;
            font-size: 14px;
        }

        .search-box input:focus {
            outline: none;
            border-color: #0d6efd;
        }

        #fileExplorer {
            display: grid;
            grid-template-columns: repeat(auto-fill,minmax(180px,1fr));
            gap: 12px;
        }

        .item {
            background: white;
            border: 1px solid #ddd;
            border-radius: 8px;
            min-height: 120px;
            padding: 15px;
            cursor: pointer;
            transition: .2s;
            text-decoration: none;
            color: inherit;

            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
        }

        .item:hover {
            background: #eef6ff;
            border-color: #0d6efd;
        }

        .icon {
            font-size: 42px;
            margin-bottom: 10px;
        }

        .name {
            text-align: center;
            word-break: break-word;
            font-size: 13px;
        }

        .back-btn {
            display: inline-block;
            margin-bottom: 15px;
            cursor: pointer;
            color: #0d6efd;
            font-weight: bold;
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="toolbar">

            <div class="search-box">
                <input type="text"
                       id="txtSearch"
                       placeholder="Search Order Number..."
                       onkeyup="filterFolders();" />
            </div>

            <div id="breadcrumb" class="breadcrumb">
                Order Files
            </div>

        </div>

        <div id="fileExplorer"></div>
    </form>
    <script>
        $(document).ready(function () {
            loadFolders();
        });

        function loadFolders() {
            $("#breadcrumb").html("Order Files");
            $(".search-box").show();
            $.ajax({
                type: "POST",
                url: "FileManager.aspx/GetFolders",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    let html = '';

                    $.each(res.d, function (i, item) {
                        html += `
                            <div class="item folder-item"
                                 data-folder="${item.Name}"
                                 onclick="loadFiles('${item.Name}')">

                                <div class="icon">📁</div>

                                <div class="name">
                                    ${item.Name}
                                </div>

                            </div>
                        `;
                    });

                    $("#fileExplorer").html(html);
                    filterFolders();
                }
            });

        }

        function loadFiles(folderName) {
            $("#breadcrumb").html(
                `Order Files / ${folderName}`
            );

            $(".search-box").hide();

            $.ajax({
                type: "POST",
                url: "FileManager.aspx/GetFiles",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    folderName: folderName
                }),
                dataType: "json",
                success: function (res) {

                    let html = `
                        <div class="back-btn"
                             onclick="loadFolders();">
                            ← Back
                        </div>
                    `;

                    $.each(res.d, function (i, item) {

                        let icon = getFileIcon(item.Name);

                        html += `
                            <a href="${item.Url}"
                               target="_blank"
                               class="item">

                                <div class="icon">
                                    ${icon}
                                </div>

                                <div class="name">
                                    ${item.Name}
                                </div>

                            </a>
                        `;
                    });

                    $("#fileExplorer").html(html);

                }
            });

        }

        function filterFolders() {
            let keyword = $("#txtSearch")
                .val()
                .toLowerCase()
                .trim();

            $(".folder-item").each(function () {

                let folderName = $(this)
                    .data("folder")
                    .toLowerCase();

                if (folderName.indexOf(keyword) > -1) {
                    $(this).show();
                }
                else {
                    $(this).hide();
                }

            });

        }

        function getFileIcon(fileName) {

            let ext = fileName.split('.').pop().toLowerCase();

            switch (ext) {

                case "pdf":
                    return "📕";

                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                case "bmp":
                case "webp":
                    return "🖼️";

                case "xls":
                case "xlsx":
                case "csv":
                    return "📊";

                case "doc":
                case "docx":
                    return "📝";

                case "ppt":
                case "pptx":
                    return "📽️";

                case "zip":
                case "rar":
                case "7z":
                    return "📦";

                case "txt":
                    return "📄";

                default:
                    return "📄";
            }

        }

    </script>
</body>
</html>
