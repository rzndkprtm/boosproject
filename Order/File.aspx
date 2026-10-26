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
        var currentPage = 1;
        var pageSize = 100;
        var totalItems = 0;
        var totalPages = 0;
        var currentSearch = "";
        var searchTimer = null;

        var fileRequestUrl = '<%= ResolveUrl("~/Order/File.aspx") %>';

        document.addEventListener("DOMContentLoaded", function () {
            loadFiles("", 1);

            var searchInput = document.getElementById("txtSearchFile");

            if (searchInput) {
                searchInput.addEventListener("input", function () {
                    clearTimeout(searchTimer);

                    searchTimer = setTimeout(function () {
                        currentSearch = searchInput.value.trim();
                        loadFiles(currentPath, 1);
                    }, 300);
                });
            }

            var previewModal = document.getElementById("filePreviewModal");

            if (previewModal) {
                previewModal.addEventListener("hidden.bs.modal", function () {
                    var previewFrame = document.getElementById("previewFrame");

                    if (previewFrame) {
                        previewFrame.src = "about:blank";
                    }
                });
            }
        });

        function loadFiles(path, page) {
            currentPath = path || "";
            currentPage = page || 1;

            showFileMessage("Loading files...", false);

            fetch(fileRequestUrl + "/GetFiles", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json; charset=utf-8"
                },
                body: JSON.stringify({
                    path: currentPath,
                    page: currentPage,
                    pageSize: pageSize,
                    search: currentSearch
                })
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error("Failed to load files. HTTP " + response.status);
                    }

                    return response.json();
                })
                .then(function (response) {
                    var result = response.d || response;

                    if (!result || result.Success === false) {
                        throw new Error(
                            result && result.Message
                                ? result.Message
                                : "Failed to load files."
                        );
                    }

                    currentPath = result.CurrentPath || "";
                    currentPage = result.CurrentPage || 1;
                    pageSize = result.PageSize || pageSize;
                    totalItems = result.TotalItems || 0;
                    totalPages = result.TotalPages || 0;
                    allFiles = result.Files || [];

                    renderBreadcrumb();
                    renderFiles();
                    renderPagination();
                    hideFileMessage();
                })
                .catch(function (error) {
                    console.error("Error loading files:", error);

                    allFiles = [];
                    totalItems = 0;
                    totalPages = 0;

                    renderFiles();
                    renderPagination();
                    showFileMessage(error.message || "Unable to load files.", true);
                });
        }

        function renderBreadcrumb() {
            var breadcrumb = document.getElementById("fileBreadcrumb");

            if (!breadcrumb) {
                return;
            }

            breadcrumb.innerHTML = "";

            var rootItem = document.createElement("li");
            rootItem.className = "breadcrumb-item";

            var rootLink = document.createElement("a");
            rootLink.href = "javascript:void(0)";
            rootLink.textContent = "Order Files";

            rootLink.addEventListener("click", function () {
                resetSearch();
                loadFiles("", 1);
            });

            rootItem.appendChild(rootLink);
            breadcrumb.appendChild(rootItem);

            if (!currentPath) {
                rootItem.classList.add("active");
                return;
            }

            var pathParts = currentPath.split(/[\\/]/).filter(function (part) {
                return part !== "";
            });

            var accumulatedPath = "";

            pathParts.forEach(function (part, index) {
                accumulatedPath = accumulatedPath
                    ? accumulatedPath + "/" + part
                    : part;

                var item = document.createElement("li");
                item.className = "breadcrumb-item";

                if (index === pathParts.length - 1) {
                    item.classList.add("active");
                    item.textContent = part;
                } else {
                    var link = document.createElement("a");
                    link.href = "javascript:void(0)";
                    link.textContent = part;

                    (function (folderPath) {
                        link.addEventListener("click", function () {
                            resetSearch();
                            loadFiles(folderPath, 1);
                        });
                    })(accumulatedPath);

                    item.appendChild(link);
                }

                breadcrumb.appendChild(item);
            });
        }

        function renderFiles() {
            var fileList = document.getElementById("fileList");
            var fileEmpty = document.getElementById("fileEmpty");
            var fileCount = document.getElementById("fileCount");

            if (!fileList) {
                return;
            }

            fileList.innerHTML = "";

            var files = allFiles || [];

            if (files.length === 0) {
                if (fileEmpty) {
                    fileEmpty.style.display = "";
                    fileEmpty.textContent = currentSearch
                        ? "No files or folders match your search."
                        : "This folder is empty.";
                }

                if (fileCount) {
                    fileCount.textContent = "0 items";
                }

                return;
            }

            if (fileEmpty) {
                fileEmpty.style.display = "none";
            }

            files.forEach(function (file) {
                var row = document.createElement("tr");

                var nameCell = document.createElement("td");
                var nameContainer = document.createElement("div");
                nameContainer.className = "d-flex align-items-center";

                var icon = document.createElement("i");
                icon.className = getFileIcon(file);
                icon.style.marginRight = "10px";
                icon.style.fontSize = "1.2rem";

                var nameLink = document.createElement("a");
                nameLink.href = "javascript:void(0)";
                nameLink.textContent = file.Name || "";
                nameLink.className = "text-decoration-none";

                if (file.IsDirectory) {
                    nameLink.addEventListener("click", function () {
                        resetSearch();
                        loadFiles(file.RelativePath, 1);
                    });
                } else {
                    nameLink.addEventListener("click", function () {
                        previewFile(file);
                    });
                }

                nameContainer.appendChild(icon);
                nameContainer.appendChild(nameLink);
                nameCell.appendChild(nameContainer);
                row.appendChild(nameCell);

                var typeCell = document.createElement("td");
                typeCell.textContent = file.IsDirectory
                    ? "Folder"
                    : (file.Extension || "File").replace(".", "").toUpperCase();

                row.appendChild(typeCell);

                var sizeCell = document.createElement("td");
                sizeCell.textContent = file.IsDirectory
                    ? "-"
                    : formatBytes(file.Size || 0);

                row.appendChild(sizeCell);

                var modifiedCell = document.createElement("td");
                modifiedCell.textContent = file.Modified || "-";
                row.appendChild(modifiedCell);

                var actionCell = document.createElement("td");
                actionCell.className = "text-end";

                if (file.IsDirectory) {
                    var openButton = document.createElement("button");
                    openButton.type = "button";
                    openButton.className = "btn btn-sm btn-outline-primary";
                    openButton.innerHTML = '<i class="bi bi-folder2-open"></i> Open';

                    openButton.addEventListener("click", function () {
                        resetSearch();
                        loadFiles(file.RelativePath, 1);
                    });

                    actionCell.appendChild(openButton);
                } else {
                    if (isPreviewable(file)) {
                        var previewButton = document.createElement("button");
                        previewButton.type = "button";
                        previewButton.className = "btn btn-sm btn-outline-secondary me-1";
                        previewButton.title = "Preview";
                        previewButton.innerHTML = '<i class="bi bi-eye"></i>';

                        previewButton.addEventListener("click", function () {
                            previewFile(file);
                        });

                        actionCell.appendChild(previewButton);
                    }

                    var downloadButton = document.createElement("button");
                    downloadButton.type = "button";
                    downloadButton.className = "btn btn-sm btn-outline-primary";
                    downloadButton.title = "Download";
                    downloadButton.innerHTML = '<i class="bi bi-download"></i>';

                    downloadButton.addEventListener("click", function () {
                        downloadFile(file);
                    });

                    actionCell.appendChild(downloadButton);
                }

                row.appendChild(actionCell);
                fileList.appendChild(row);
            });

            if (fileCount) {
                var startItem = ((currentPage - 1) * pageSize) + 1;
                var endItem = Math.min(currentPage * pageSize, totalItems);

                fileCount.textContent =
                    "Showing " + startItem + "-" + endItem +
                    " of " + totalItems + " items";
            }
        }

        function renderPagination() {
            var pagination = document.getElementById("filePagination");

            if (!pagination) {
                return;
            }

            pagination.innerHTML = "";

            if (totalPages <= 1) {
                return;
            }

            addPageControl(
                pagination,
                "Previous",
                currentPage > 1,
                function () {
                    loadFiles(currentPath, currentPage - 1);
                }
            );

            var startPage = Math.max(1, currentPage - 2);
            var endPage = Math.min(totalPages, currentPage + 2);

            if (startPage > 1) {
                addPageButton(pagination, 1);

                if (startPage > 2) {
                    addEllipsis(pagination);
                }
            }

            for (var page = startPage; page <= endPage; page++) {
                addPageButton(pagination, page);
            }

            if (endPage < totalPages) {
                if (endPage < totalPages - 1) {
                    addEllipsis(pagination);
                }

                addPageButton(pagination, totalPages);
            }

            addPageControl(
                pagination,
                "Next",
                currentPage < totalPages,
                function () {
                    loadFiles(currentPath, currentPage + 1);
                }
            );
        }

        function addPageControl(container, label, enabled, callback) {
            var item = document.createElement("li");
            item.className = "page-item";

            if (!enabled) {
                item.classList.add("disabled");
            }

            var link = document.createElement("a");
            link.className = "page-link";
            link.href = "javascript:void(0)";
            link.textContent = label;

            if (enabled) {
                link.addEventListener("click", callback);
            }

            item.appendChild(link);
            container.appendChild(item);
        }

        function addPageButton(container, page) {
            var item = document.createElement("li");
            item.className = "page-item";

            if (page === currentPage) {
                item.classList.add("active");
            }

            var link = document.createElement("a");
            link.className = "page-link";
            link.href = "javascript:void(0)";
            link.textContent = page;

            link.addEventListener("click", function () {
                if (page !== currentPage) {
                    loadFiles(currentPath, page);
                }
            });

            item.appendChild(link);
            container.appendChild(item);
        }

        function addEllipsis(container) {
            var item = document.createElement("li");
            item.className = "page-item disabled";

            var span = document.createElement("span");
            span.className = "page-link";
            span.textContent = "...";

            item.appendChild(span);
            container.appendChild(item);
        }

        function resetSearch() {
            currentSearch = "";
            clearTimeout(searchTimer);

            var searchInput = document.getElementById("txtSearchFile");

            if (searchInput) {
                searchInput.value = "";
            }
        }

        function getFileIcon(file) {
            if (file.IsDirectory) {
                return "bi bi-folder-fill text-warning";
            }

            switch ((file.Extension || "").toLowerCase()) {
                case ".pdf":
                    return "bi bi-file-earmark-pdf text-danger";

                case ".doc":
                case ".docx":
                    return "bi bi-file-earmark-word text-primary";

                case ".xls":
                case ".xlsx":
                case ".csv":
                    return "bi bi-file-earmark-excel text-success";

                case ".ppt":
                case ".pptx":
                    return "bi bi-file-earmark-ppt text-danger";

                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                case ".bmp":
                case ".webp":
                case ".svg":
                    return "bi bi-file-earmark-image text-info";

                case ".txt":
                case ".log":
                    return "bi bi-file-earmark-text";

                case ".zip":
                case ".rar":
                case ".7z":
                    return "bi bi-file-earmark-zip";

                default:
                    return "bi bi-file-earmark";
            }
        }

        function isPreviewable(file) {
            if (!file || file.IsDirectory) {
                return false;
            }

            var extensions = [
                ".pdf",
                ".jpg",
                ".jpeg",
                ".png",
                ".gif",
                ".bmp",
                ".webp",
                ".svg",
                ".txt"
            ];

            return extensions.indexOf(
                (file.Extension || "").toLowerCase()
            ) !== -1;
        }

        function previewFile(file) {
            if (!file || file.IsDirectory || !file.RelativePath) {
                return;
            }

            var previewUrl = fileRequestUrl +
                "?action=view&path=" +
                encodeURIComponent(file.RelativePath);

            var previewFrame = document.getElementById("previewFrame");
            var previewTitle = document.getElementById("previewTitle");
            var modalElement = document.getElementById("filePreviewModal");

            if (!previewFrame || !modalElement) {
                window.open(previewUrl, "_blank");
                return;
            }

            if (previewTitle) {
                previewTitle.textContent = file.Name || "File Preview";
            }

            previewFrame.src = previewUrl;

            if (window.bootstrap && bootstrap.Modal) {
                var modal = bootstrap.Modal.getInstance(modalElement);

                if (!modal) {
                    modal = new bootstrap.Modal(modalElement);
                }

                modal.show();
            } else if (window.jQuery && jQuery.fn.modal) {
                jQuery(modalElement).modal("show");
            } else {
                window.open(previewUrl, "_blank");
            }
        }

        function downloadFile(file) {
            if (!file || file.IsDirectory || !file.RelativePath) {
                showFileMessage("Invalid file path.", true);
                return;
            }

            var downloadUrl = fileRequestUrl +
                "?action=download&path=" +
                encodeURIComponent(file.RelativePath);

            window.location.href = downloadUrl;
        }

        function formatBytes(bytes) {
            bytes = Number(bytes) || 0;

            if (bytes === 0) {
                return "0 Bytes";
            }

            var units = ["Bytes", "KB", "MB", "GB", "TB"];
            var index = Math.floor(Math.log(bytes) / Math.log(1024));

            index = Math.min(index, units.length - 1);

            var size = bytes / Math.pow(1024, index);

            return size.toFixed(index === 0 ? 0 : 2) + " " + units[index];
        }

        function showFileMessage(message, isError) {
            var fileEmpty = document.getElementById("fileEmpty");

            if (!fileEmpty) {
                return;
            }

            fileEmpty.style.display = "";
            fileEmpty.textContent = message;

            if (isError) {
                fileEmpty.classList.add("text-danger");
            } else {
                fileEmpty.classList.remove("text-danger");
            }
        }

        function hideFileMessage() {
            var fileEmpty = document.getElementById("fileEmpty");

            if (fileEmpty) {
                fileEmpty.style.display = "none";
                fileEmpty.classList.remove("text-danger");
            }
        }
    </script>
</asp:Content>