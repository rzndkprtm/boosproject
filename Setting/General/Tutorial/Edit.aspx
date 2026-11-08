<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Edit.aspx.vb" Inherits="Setting_General_Tutorial_Edit" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" ValidateRequest="false" Title="Edit Tutorial" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .tutorial-wrapper { width: 100%; max-width: 100%; }
        .tutorial-card { border: 0; border-radius: 14px; box-shadow: 0 4px 24px rgba(0,0,0,.055); overflow: hidden; }
        .tutorial-card .card-header { background: #fff; border-bottom: 1px solid #f0f0f0; padding: 24px 28px; }
        .tutorial-card .card-body { padding: 28px; }
        .tutorial-card .card-footer { background: #fff; border-top: 1px solid #f0f0f0; padding: 20px 28px; }
        .tutorial-heading { display: flex; align-items: center; gap: 14px; }
        .tutorial-icon { width: 46px; height: 46px; border-radius: 12px; background: #eef3ff; color: #435ebe; display: flex; align-items: center; justify-content: center; font-size: 21px; flex-shrink: 0; }
        .tutorial-heading h4 { margin: 0 0 5px; font-size: 17px; font-weight: 700; color: #25396f; }
        .tutorial-heading p { margin: 0; font-size: 12px; color: #8a8a8a; }
        .tutorial-section { margin-bottom: 25px; }
        .tutorial-section:last-child { margin-bottom: 0; }
        .tutorial-card .form-label { font-size: 13px; font-weight: 600; color: #344054; margin-bottom: 9px; }
        .tutorial-card .form-control, .tutorial-card .form-select { min-height: 44px; border-radius: 8px; border: 1px solid #dfe3eb; font-size: 13px; padding: 10px 13px; transition: all .2s ease; }
        .tutorial-card .form-control:focus, .tutorial-card .form-select:focus { border-color: #435ebe; box-shadow: 0 0 0 3px rgba(67,94,190,.10); }
        .tutorial-card .form-control::placeholder { color: #b0b7c3; }
        .tutorial-hint { font-size: 11px; color: #9299a5; margin-top: 7px; line-height: 1.5; }
        .tutorial-divider { border: 0; border-top: 1px solid #f0f1f4; margin: 26px 0; }
        .tutorial-subheading { font-size: 13px; font-weight: 700; color: #344054; margin-bottom: 16px; }
        .tutorial-editor { border-radius: 9px; overflow: hidden; }
        .tutorial-editor .note-editor { border: 1px solid #dfe3eb !important; border-radius: 9px !important; }
        .tutorial-editor .note-toolbar { background: #f8f9fc !important; border-bottom: 1px solid #edf0f5 !important; }
        .tutorial-editor .note-statusbar { border-top: 1px solid #edf0f5 !important; }
        .tutorial-file-box { background: #fafbfe; border: 1px dashed #d5dbe7; border-radius: 10px; padding: 16px; height: 100%; }
        .tutorial-file-box .form-control { background: #fff; }
        .tutorial-update-box { background: #f5f7ff; border: 1px solid #e5eaff; border-radius: 10px; padding: 18px; }
        .tutorial-update-box .form-select { background-color: #fff; }
        .tutorial-status-box { max-width: 260px; }
        .tutorial-card .btn { border-radius: 8px; padding: 10px 24px; font-size: 13px; font-weight: 600; min-width: 110px; }
        .tutorial-card .btn-primary { background: #435ebe; border-color: #435ebe; }
        .tutorial-card .btn-primary:hover { background: #354ca0; border-color: #354ca0; }
        .tutorial-card .btn-danger { background: #fff; color: #dc3545; border: 1px solid #f1c5ca; }
        .tutorial-card .btn-danger:hover { background: #fff2f3; }
        .tutorial-error { border-radius: 9px; font-size: 13px; }
        #loadingOverlay .card { border: 0; border-radius: 12px; min-width: 150px; }
        #loadingOverlay .card-body { padding: 22px 30px; font-size: 13px; color: #435ebe; font-weight: 600; }
        @media (max-width: 767px) {
            .tutorial-card .card-header { padding: 18px; }
            .tutorial-card .card-body { padding: 20px 18px; }
            .tutorial-card .card-footer { padding: 16px 18px; }
            .tutorial-card .card-footer .btn { flex: 1; }
            .tutorial-status-box { max-width: 100%; }
        }
    </style>
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted">Update and manage existing tutorial content.</p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/general">General</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/general/tutorial">Tutorial</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12 tutorial-wrapper">
                <div class="card tutorial-card">
                    <div class="card-header">
                        <div class="tutorial-heading">
                            <div class="tutorial-icon">
                                <i class="bi bi-pencil-square"></i>
                            </div>
                            <div>
                                <h4>Tutorial Information</h4>
                                <p>Update the information and resources of this tutorial.</p>
                            </div>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="form-body">
                                        <div class="mt-3 tutorial-subheading">
                                            <i class="bi bi-info-circle me-1"></i> Basic Information
                                        </div>
                                        <div class="row tutorial-section">
                                            <div class="col-12 col-lg-4 form-group mb-3">
                                                <label class="form-label">Company</label>
                                                <asp:DropDownList runat="server" ID="ddlCompanyId" CssClass="form-select"></asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-lg-8 form-group mb-3">
                                                <label class="form-label">Title</label>
                                                <asp:TextBox runat="server" ID="txtTitle" CssClass="form-control" placeholder="Enter tutorial title ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <hr class="tutorial-divider" />
                                        <div class="tutorial-subheading">
                                            <i class="bi bi-file-earmark-text me-1"></i> Tutorial Description
                                        </div>
                                        <div class="row tutorial-section">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Description</label>
                                                <div class="tutorial-editor">
                                                    <div id="description"></div>
                                                </div>
                                                <div class="tutorial-hint">Edit the tutorial content using the rich text editor.</div>
                                            </div>
                                        </div>
                                        <hr class="tutorial-divider" />
                                        <div class="tutorial-subheading">
                                            <i class="bi bi-collection-play me-1"></i> Tutorial Resources
                                        </div>
                                        <div class="row tutorial-section">
                                            <div class="col-12 col-lg-6 form-group mb-3">
                                                <div class="tutorial-update-box">
                                                    <label class="form-label">
                                                        <i class="bi bi-arrow-repeat me-1"></i> Update File / Video
                                                    </label>
                                                    <asp:DropDownList runat="server" ID="ddlUpdateFileVideo" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlUpdateFileVideo_SelectedIndexChanged">
                                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                                        <asp:ListItem Value="File" Text="File Only"></asp:ListItem>
                                                        <asp:ListItem Value="Video" Text="Video Only"></asp:ListItem>
                                                        <asp:ListItem Value="Both" Text="File & Video"></asp:ListItem>
                                                    </asp:DropDownList>
                                                    <div class="tutorial-hint">Select which tutorial resources you want to update.</div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row tutorial-section">
                                            <div class="col-12 col-lg-6 form-group mb-3" runat="server" id="divFile">
                                                <div class="tutorial-file-box">
                                                    <label class="form-label">
                                                        <i class="bi bi-file-earmark-arrow-up me-1"></i> Upload File
                                                    </label>
                                                    <asp:FileUpload runat="server" ID="fuFile" CssClass="form-control" />
                                                    <div class="tutorial-hint">Select a new file to replace the existing file.</div>
                                                </div>
                                            </div>
                                            <div class="col-12 col-lg-6 form-group mb-3" runat="server" id="divVideo">
                                                <div class="tutorial-file-box">
                                                    <label class="form-label">
                                                        <i class="bi bi-play-circle me-1"></i> Video URL
                                                    </label>
                                                    <asp:TextBox runat="server" ID="txtVideo" CssClass="form-control" placeholder="Enter video URL ..." autocomplete="off"></asp:TextBox>
                                                    <div class="tutorial-hint">Enter the new external video link, such as YouTube.</div>
                                                </div>
                                            </div>
                                        </div>
                                        <hr class="tutorial-divider" />
                                        <div class="tutorial-subheading">
                                            <i class="bi bi-gear me-1"></i> Settings
                                        </div>
                                        <div class="row tutorial-section">
                                            <div class="col-12 col-lg-5 form-group tutorial-status-box">
                                                <label class="form-label">Status</label>
                                                <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select">
                                                    <asp:ListItem Value="Active" Text="Active"></asp:ListItem>
                                                    <asp:ListItem Value="Inactive" Text="Inactive"></asp:ListItem>
                                                </asp:DropDownList>
                                                <div class="tutorial-hint">Choose whether this tutorial is available to users.</div>
                                            </div>
                                        </div>
                                        <div class="row" runat="server" id="divError">
                                            <div class="col-12">
                                                <div class="alert alert-danger tutorial-error mb-0">
                                                    <i class="bi bi-exclamation-circle me-2"></i>
                                                    <span runat="server" id="msgError"></span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div class="card-footer d-flex justify-content-end gap-2">
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" OnClientClick="return setDescriptionContent();" />
                    </div>
                </div>
            </div>
        </section>
    </div>
    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>
    <div id="loadingOverlay" style="display:none; position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(255,255,255,.5); z-index:99999;">
        <div class="position-absolute top-50 start-50 translate-middle">
            <div class="card shadow">
                <div class="card-body text-center">
                    <div class="spinner-border"></div>
                    <div class="mt-2">Loading...</div>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField runat="server" ID="fieldMessage" />
    <asp:HiddenField runat="server" ID="hfMessage" />
    <script type="text/javascript">
        var summernoteInitialized = false;
        function initSummernote() {
            var editor = $('#description');

            if (!editor.length) return;
            if (editor.next('.note-editor').length) {
                summernoteInitialized = true;
                return;
            }
            editor.summernote({
                tabsize: 2,
                height: 350,
                callbacks: {
                    onChange: function (contents) {
                        $('#<%= hfMessage.ClientID %>').val(contents);
                    }
                }
            });
            editor.summernote('code', $('#<%= hfMessage.ClientID %>').val());
            summernoteInitialized = true;
        }
        function destroySummernote() {
            var editor = $('#description');
            if (editor.length && editor.next('.note-editor').length) {
                $('#<%= hfMessage.ClientID %>').val(editor.summernote('code'));
                editor.summernote('destroy');
            }
            summernoteInitialized = false;
        }
        function setDescriptionContent() {
            var editor = $('#description');
            if (editor.length && editor.next('.note-editor').length) {
                var content = editor.summernote('code');
                $('#<%= fieldMessage.ClientID %>').val(content);
            }
            return true;
        }
        function initUpdatePanelLoading() {
            if (typeof Sys === "undefined") return;

            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_beginRequest(function () {
                destroySummernote();

                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "block";
            });

            prm.add_endRequest(function () {
                initSummernote();

                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "none";
            });
        }
        $(document).ready(function () {
            initSummernote();
            initUpdatePanelLoading();
        });
        window.addEventListener("pageshow", function () {
            var loading = document.getElementById("loadingOverlay");
            if (loading) loading.style.display = "none";
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>