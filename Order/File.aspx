<%@ Page Language="VB" AutoEventWireup="false" CodeFile="File.aspx.vb" Inherits="Order_File" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Order File" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted">Manage and browse your files.</p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <div class="d-flex justify-content-between align-items-center flex-wrap gap-2">
                            <h5 class="card-title mb-0">
                                <i class="bi bi-folder2-open me-2"></i>File Explorer
                            </h5>
                            <button type="button" class="btn btn-sm btn-outline-primary" onclick="loadFiles(currentPath)">
                                <i class="bi bi-arrow-clockwise me-1"></i>Refresh
                            </button>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row align-items-center mb-3">
                            <div class="col-md-7 mb-2 mb-md-0">
                                <nav aria-label="breadcrumb">
                                    <ol class="breadcrumb mb-0" id="fileBreadcrumb">
                                        <li class="breadcrumb-item active">File</li>
                                    </ol>
                                </nav>
                            </div>
                            <div class="col-md-5">
                                <div class="input-group">
                                    <span class="input-group-text">
                                        <i class="bi bi-search"></i>
                                    </span>
                                    <input type="text" id="txtSearchFile" class="form-control" placeholder="Search files and folders..." />
                                </div>
                            </div>
                        </div>
                        <div id="fileMessage" class="alert d-none" role="alert"></div>
                        <div class="table-responsive">
                            <table class="table table-hover align-middle" id="tblFiles">
                                <thead>
                                    <tr>
                                        <th style="width: 55%;">Name</th>
                                        <th>Type</th>
                                        <th>Size</th>
                                        <th>Modified</th>
                                        <th class="text-end">Action</th>
                                    </tr>
                                </thead>
                                <tbody id="fileList">
                                    <tr>
                                        <td colspan="5" class="text-center py-4">
                                            <div class="spinner-border spinner-border-sm text-primary me-2" role="status"></div>
                                            Loading files...
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div id="fileEmpty" class="text-center py-5 d-none">
                            <i class="bi bi-folder2-open display-4 text-muted"></i>
                            <h6 class="mt-3">This folder is empty</h6>
                            <p class="text-muted mb-0">There are no files or folders to display.</p>
                        </div>
                        <div class="d-flex justify-content-between align-items-center mt-3">
                            <small class="text-muted" id="fileCount">Loading...</small>
                            <small class="text-muted">Directory: ~/File/</small>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
    
    <div class="modal fade" id="filePreviewModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-centered" style="max-width: 1100px;">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="previewTitle">File Preview</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body p-0" style="height: 75vh;">
                    <iframe id="previewFrame" src="about:blank" style="width: 100%; height: 100%; border: 0;" title="File Preview"></iframe>
                </div>
            </div>
        </div>
    </div>

    <style>
        #tblFiles thead th {
            white-space: nowrap;
            font-size: 0.85rem;
            color: #607080;
        }

        #tblFiles tbody td {
            font-size: 0.9rem;
        }

        .file-name {
            display: flex;
            align-items: center;
            gap: 12px;
            min-width: 220px;
        }

        .file-icon {
            width: 40px;
            height: 40px;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.35rem;
            flex-shrink: 0;
            background: #f1f5f9;
            color: #607080;
        }

        .file-icon.folder {
            background: #fff4d6;
            color: #d99b00;
        }

        .file-name-link {
            color: #25396f;
            text-decoration: none;
            font-weight: 600;
            cursor: pointer;
            overflow-wrap: anywhere;
        }

        .file-name-link:hover {
            color: #435ebe;
            text-decoration: underline;
        }

        .file-subtitle {
            font-size: 0.75rem;
            color: #8994a3;
            margin-top: 2px;
        }

        .file-actions {
            display: flex;
            justify-content: flex-end;
            gap: 5px;
            white-space: nowrap;
        }

        #fileBreadcrumb .breadcrumb-item a {
            text-decoration: none;
            cursor: pointer;
        }

        #fileBreadcrumb .breadcrumb-item a:hover {
            text-decoration: underline;
        }

        #fileSearchEmpty {
            color: #8994a3;
        }
    </style>

    <script type="text/javascript">
        var currentPath = "";
        var allFiles = [];
        var fileRequestUrl = '<%= ResolveUrl("~/Order/File.aspx") %>';

        document.addEventListener("DOMContentLoaded", function () {
            loadFiles("");

            document.getElementById("txtSearchFile").addEventListener("input", function () {
                renderFiles();
            });

            var previewModal = document.getElementById("filePreviewModal");
            previewModal.addEventListener("hidden.bs.modal", function () {
                document.getElementById("previewFrame").src = "about:blank";
            });
        });

        function loadFiles(path) {
            currentPath = path || "";

            var tbody = document.getElementById("fileList");
            tbody.innerHTML = '<tr><td colspan="5" class="text-center py-4"><div class="spinner-border spinner-border-sm text-primary me-2"></div>Loading files...</td></tr>';

            document.getElementById("fileEmpty").classList.add("d-none");
            document.getElementById("fileCount").textContent = "Loading...";
            hideFileMessage();

            fetch(fileRequestUrl + "/GetFiles", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json; charset=utf-8"
                },
                body: JSON.stringify({
                    path: currentPath,
                    page: 1,
                    pageSize: 100
                })
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error("Unable to load files.");
                    }
                    return response.json();
                })
                .then(function (result) {
                    var data = result.d;

                    if (!data || !data.Success) {
                        allFiles = [];
                        renderBreadcrumb();
                        renderFiles();
                        showFileMessage(data ? data.Message : "Unable to load files.", "danger");
                        return;
                    }

                    currentPath = data.CurrentPath || "";
                    allFiles = data.Files || [];

                    renderBreadcrumb();
                    renderFiles();
                })
                .catch(function (error) {
                    allFiles = [];
                    document.getElementById("fileList").innerHTML = "";
                    document.getElementById("fileCount").textContent = "Unable to load files";
                    showFileMessage("An error occurred while loading files. Please try again.", "danger");
                    console.error(error);
                });
        }

        function renderBreadcrumb() {
            var container = document.getElementById("fileBreadcrumb");
            container.innerHTML = "";

            var root = document.createElement("li");
            root.className = "breadcrumb-item";

            var rootLink = document.createElement("a");
            rootLink.href = "javascript:void(0)";
            rootLink.textContent = "File";
            rootLink.onclick = function () {
                document.getElementById("txtSearchFile").value = "";
                loadFiles("");
            };

            root.appendChild(rootLink);
            container.appendChild(root);

            if (!currentPath) {
                root.classList.add("active");
                root.removeChild(rootLink);
                root.textContent = "File";
                return;
            }

            var parts = currentPath.split("/").filter(Boolean);
            var accumulated = "";

            parts.forEach(function (part, index) {
                accumulated = accumulated ? accumulated + "/" + part : part;

                var item = document.createElement("li");
                item.className = "breadcrumb-item";

                if (index === parts.length - 1) {
                    item.classList.add("active");
                    item.textContent = part;
                } else {
                    var link = document.createElement("a");
                    link.href = "javascript:void(0)";
                    link.textContent = part;

                    (function (targetPath) {
                        link.onclick = function () {
                            document.getElementById("txtSearchFile").value = "";
                            loadFiles(targetPath);
                        };
                    })(accumulated);

                    item.appendChild(link);
                }

                container.appendChild(item);
            });
        }

        function renderFiles() {
            var tbody = document.getElementById("fileList");
            var keyword = document.getElementById("txtSearchFile").value.trim().toLowerCase();

            tbody.innerHTML = "";

            var filteredFiles = allFiles.filter(function (file) {
                return !keyword || file.Name.toLowerCase().includes(keyword);
            });

            filteredFiles.sort(function (a, b) {
                if (a.IsDirectory !== b.IsDirectory) {
                    return a.IsDirectory ? -1 : 1;
                }
                return a.Name.localeCompare(b.Name);
            });

            if (filteredFiles.length === 0) {
                document.getElementById("fileEmpty").classList.remove("d-none");
                document.getElementById("fileCount").textContent =
                    keyword ? "No matching files or folders" : "0 items";

                document.getElementById("fileEmpty").querySelector("h6").textContent =
                    keyword ? "No matching files found" : "This folder is empty";

                document.getElementById("fileEmpty").querySelector("p").textContent =
                    keyword ? "Try a different search term." : "There are no files or folders to display.";

                return;
            }

            document.getElementById("fileEmpty").classList.add("d-none");

            filteredFiles.forEach(function (file) {
                var row = document.createElement("tr");

                // Name
                var nameCell = document.createElement("td");
                var nameContainer = document.createElement("div");
                nameContainer.className = "file-name";

                var icon = document.createElement("div");
                icon.className = "file-icon" + (file.IsDirectory ? " folder" : "");
                icon.innerHTML = file.IsDirectory
                    ? '<i class="bi bi-folder-fill"></i>'
                    : getFileIcon(file.Extension);

                var nameInfo = document.createElement("div");
                var nameLink = document.createElement("a");
                nameLink.className = "file-name-link";
                nameLink.href = "javascript:void(0)";
                nameLink.textContent = file.Name;

                if (file.IsDirectory) {
                    nameLink.onclick = function () {
                        document.getElementById("txtSearchFile").value = "";
                        loadFiles(file.RelativePath);
                    };
                } else if (isPreviewable(file.Extension)) {
                    nameLink.onclick = function () {
                        previewFile(file);
                    };
                } else {
                    nameLink.onclick = function () {
                        downloadFile(file.RelativePath);
                    };
                }

                var subtitle = document.createElement("div");
                subtitle.className = "file-subtitle";
                subtitle.textContent = file.IsDirectory ? "Folder" : formatBytes(file.Size);

                nameInfo.appendChild(nameLink);
                nameInfo.appendChild(subtitle);
                nameContainer.appendChild(icon);
                nameContainer.appendChild(nameInfo);
                nameCell.appendChild(nameContainer);

                // Type
                var typeCell = document.createElement("td");
                typeCell.textContent = file.IsDirectory ? "Folder" : (file.Extension ? file.Extension.toUpperCase().replace(".", "") + " File" : "File");

                // Size
                var sizeCell = document.createElement("td");
                sizeCell.textContent = file.IsDirectory ? "—" : formatBytes(file.Size);

                // Modified
                var modifiedCell = document.createElement("td");
                modifiedCell.textContent = file.Modified || "—";

                // Actions
                var actionCell = document.createElement("td");
                actionCell.className = "text-end";

                var actions = document.createElement("div");
                actions.className = "file-actions";

                if (file.IsDirectory) {
                    var openButton = document.createElement("button");
                    openButton.type = "button";
                    openButton.className = "btn btn-sm btn-light-primary";
                    openButton.title = "Open folder";
                    openButton.innerHTML = '<i class="bi bi-folder2-open"></i>';
                    openButton.onclick = function () {
                        document.getElementById("txtSearchFile").value = "";
                        loadFiles(file.RelativePath);
                    };
                    actions.appendChild(openButton);
                } else {
                    if (isPreviewable(file.Extension)) {
                        var previewButton = document.createElement("button");
                        previewButton.type = "button";
                        previewButton.className = "btn btn-sm btn-light-primary";
                        previewButton.title = "Preview";
                        previewButton.innerHTML = '<i class="bi bi-eye"></i>';
                        previewButton.onclick = function () {
                            previewFile(file);
                        };
                        actions.appendChild(previewButton);
                    }

                    var downloadButton = document.createElement("button");
                    downloadButton.type = "button";
                    downloadButton.className = "btn btn-sm btn-light-success";
                    downloadButton.title = "Download";
                    downloadButton.innerHTML = '<i class="bi bi-download"></i>';
                    downloadButton.onclick = function () {
                        downloadFile(file.RelativePath);
                    };
                    actions.appendChild(downloadButton);
                }

                actionCell.appendChild(actions);

                row.appendChild(nameCell);
                row.appendChild(typeCell);
                row.appendChild(sizeCell);
                row.appendChild(modifiedCell);
                row.appendChild(actionCell);

                tbody.appendChild(row);
            });

            document.getElementById("fileCount").textContent =
                filteredFiles.length + (filteredFiles.length === 1 ? " item" : " items");
        }

        function getFileIcon(extension) {
            switch ((extension || "").toLowerCase()) {
                case ".pdf":
                    return '<i class="bi bi-file-earmark-pdf-fill text-danger"></i>';
                case ".doc":
                case ".docx":
                    return '<i class="bi bi-file-earmark-word-fill text-primary"></i>';
                case ".xls":
                case ".xlsx":
                case ".csv":
                    return '<i class="bi bi-file-earmark-excel-fill text-success"></i>';
                case ".ppt":
                case ".pptx":
                    return '<i class="bi bi-file-earmark-ppt-fill text-warning"></i>';
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                case ".webp":
                case ".bmp":
                case ".svg":
                    return '<i class="bi bi-file-earmark-image-fill text-info"></i>';
                case ".zip":
                case ".rar":
                case ".7z":
                    return '<i class="bi bi-file-earmark-zip-fill text-secondary"></i>';
                case ".txt":
                case ".log":
                    return '<i class="bi bi-file-earmark-text-fill text-secondary"></i>';
                default:
                    return '<i class="bi bi-file-earmark-fill"></i>';
            }
        }

        function isPreviewable(extension) {
            return [".pdf", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg", ".txt"].includes((extension || "").toLowerCase());
        }

        function previewFile(file) {
            document.getElementById("previewTitle").textContent = file.Name;
            document.getElementById("previewFrame").src =
                fileRequestUrl + "?action=view&path=" + encodeURIComponent(file.RelativePath);

            var modalElement = document.getElementById("filePreviewModal");
            var modalInstance = bootstrap.Modal.getInstance(modalElement);

            if (!modalInstance) {
                modalInstance = new bootstrap.Modal(modalElement);
            }

            modalInstance.show();
        }

        function downloadFile(path) {
            window.location.href =
                fileRequestUrl + "?action=download&path=" + encodeURIComponent(path);
        }

        function formatBytes(bytes) {
            if (!bytes || bytes <= 0) return "—";
            var units = ["B", "KB", "MB", "GB", "TB"];
            var index = Math.floor(Math.log(bytes) / Math.log(1024));
            index = Math.min(index, units.length - 1);
            return (bytes / Math.pow(1024, index)).toFixed(index === 0 ? 0 : 2) + " " + units[index];
        }

        function showFileMessage(message, type) {
            var element = document.getElementById("fileMessage");
            element.className = "alert alert-" + type;
            element.textContent = message;
        }

        function hideFileMessage() {
            var element = document.getElementById("fileMessage");
            element.className = "alert d-none";
            element.textContent = "";
        }
    </script>
</asp:Content>