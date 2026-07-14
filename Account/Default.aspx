<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Account_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="My Account" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .profile-card{ border:none; border-radius:16px; box-shadow:0 4px 20px rgba(0,0,0,.06); overflow:hidden; height:100%; }
        .profile-card .card-header{ background:#fff; border-bottom:1px solid #edf2f7; padding:20px 24px; }
        .profile-card .card-title{ margin:0; font-size:20px; font-weight:600; color:#1e293b; }
        .profile-card .card-subtitle{ color:#64748b; font-size:13px; margin-top:4px; }
        .profile-card .card-body{ padding:24px; }
        .info-row{ display:flex; justify-content:space-between; align-items:center; padding:18px 0; border-bottom:1px solid #f1f5f9; }
        .info-row:last-child{ border-bottom:none; }
        .info-label{ font-size:13px; color:#64748b; margin-bottom:4px; }
        .info-value{ font-size:16px; font-weight:600; color:#0f172a; }
        .edit-link{ border:none; background:#eff6ff; color:#2563eb; padding:6px 10px; border-radius:8px; text-decoration:none; }
        .edit-link:hover{ background:#dbeafe; }
        .section-title{ font-size:16px; font-weight:600; margin-bottom:15px; color:#1e293b; }
        .summary-box{ background:#f8fafc; border:1px solid #e2e8f0; border-radius:12px; padding:18px; margin-bottom:20px; }
        .note-box{ margin-top:20px; background:#f8fafc; border-left:4px solid #2563eb; padding:14px 16px; border-radius:10px; color:#475569; font-size:14px; }
        .note-box i{ margin-right:8px; color:#2563eb; }
        .table-responsive{ border:1px solid #e2e8f0; border-radius:12px; overflow-x:auto; overflow-y:hidden; }
        .table{ margin-bottom:0 !important; table-layout:fixed; width:100%; }
        .table th, .table td{ vertical-align:middle; word-break:break-word; }
        .table th:nth-child(1){ width:60px; }
        .table th:nth-child(2){ width:25%; }
        .table th:nth-child(3){ width:45%; }
        .table th:nth-child(4){ width:30%; }
        .empty-contact{ text-align:center; padding:30px; color:#94a3b8; }
    </style>

    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted"></p>
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
        <section class="row mb-3" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-5 mb-3">
                <div class="card profile-card">
                    <div class="card-header">
                        <div class="card-title">Personal Account</div>
                        <div class="card-subtitle">Your personal account information</div>
                    </div>
                    <div class="card-body">
                        <div class="info-row">
                            <div>
                                <div class="info-label"> Username</div>
                                <div class="info-value">
                                    <asp:Label runat="server" ID="lblUserName"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="info-row">
                            <div>
                                <div class="info-label">Full Name</div>
                                <div class="info-value">
                                    <asp:Label runat="server" ID="lblFullName"></asp:Label>
                                </div>
                            </div>
                            <a class="edit-link" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalName">
                                <i class="bi bi-pencil"></i>
                            </a>
                        </div>
                        <div class="info-row">
                            <div>
                                <div class="info-label">Email Address</div>
                                <div class="info-value">
                                    <asp:Label runat="server" ID="lblUserEmail"></asp:Label>
                                </div>
                            </div>
                            <a class="edit-link" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalEmail">
                                <i class="bi bi-pencil"></i>
                            </a>
                        </div>
                        <div class="note-box">
                            <i class="bi bi-info-circle-fill"></i>
                            These details belong only to the login account currently being used.
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-7" runat="server" id="divCompany">
                <div class="card profile-card">
                    <div class="card-header">
                        <div class="card-title">Company Account</div>
                        <div class="card-subtitle">Company and contact information</div>
                    </div>
                    <div class="card-body">
                        <div class="summary-box mt-2">
                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-8 mb-3">
                                    <div class="info-label"> Account Name</div>
                                    <div class="info-value">
                                        <asp:Label runat="server" ID="lblCustomerName"></asp:Label>
                                    </div>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-4">
                                    <div class="info-label">Sales</div>
                                    <div class="info-value">
                                        <asp:Label runat="server" ID="lblSales"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="section-title">Contact List</div>
                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvContact" CssClass="table table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="No contact information available." EmptyDataRowStyle-HorizontalAlign="Center">
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-Wrap="true" />
                                    <asp:BoundField DataField="Email" HeaderText="Email" ItemStyle-Wrap="true" />
                                    <asp:BoundField DataField="Tags" HeaderText="Tags" ItemStyle-Wrap="true" />
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="note-box">
                            <i class="bi bi-shield-lock-fill"></i>
                            Company account information is managed by IT or Accounting and cannot be modified from this page.
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade" id="modalInfo" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Personal Account (Email)</h5>
                </div>
                <div class="modal-body py-4">
                    Hi <b><%: Session("FullName") %></b>,<br /><br />
                    We have noticed that your account does not have an email address registered.<br /><br />
                    Please add your email address by navigating to <b>Personal Account → Email Address.</b><br /><br />
                    Your email address is used for account-related purposes, including password recovery and receiving login credentials or other important system notifications when required.<br /><br />
                    Thank you for your attention.<br /><br /><br />
                    Kind Regards,<br />
                    Support Team
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalName" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Update Full Name</h4>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Full Name</label>
                            <asp:TextBox runat="server" ID="txtFullName" CssClass="form-control" placeholder="Full Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorName">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorName"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnName" CssClass="btn btn-primary" Text="Submit" OnClick="btnName_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalEmail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Update Personal Email</h4>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Email</label>
                            <asp:TextBox runat="server" ID="txtUserEmail" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorEmail">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorEmail"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnEmail" CssClass="btn btn-primary" Text="Submit" OnClick="btnEmail_Click" />
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblLoginId"></asp:Label>
    </div>

    <script type="text/javascript">
        function showInfo() {
            $("#modalInfo").modal("show");
        }
        function showName() {
            $("#modalName").modal("show");
        }
        function showEmail() {
            $("#modalEmail").modal("show");
        }
        ["modalInfo", "modalName", "modalEmail"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>