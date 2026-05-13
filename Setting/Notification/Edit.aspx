<%@ Page Language="VB" AutoEventWireup="false" ValidateRequest="false" CodeFile="Edit.aspx.vb" Inherits="Setting_Notification_Edit" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Edit Notification" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/general/notification">Notification</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-8">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Edit Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <div class="row mb-3 form-group">
                                        <div class="col-12 col-sm-12 col-lg-4 mb-2">
                                            <label class="form-label">Role</label>
                                            <asp:DropDownList runat="server" ID="ddlLoginRole" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlLoginRole_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-8">
                                            <label class="form-label">Login Name</label>
                                            <asp:ListBox runat="server" ID="lbLoginId" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                        </div>
                                    </div>
                                    <div class="row mb-3 form-group">
                                        <div class="col-12 col-sm-12 col-lg-4 mb-2">
                                            <label class="form-label">Start Date</label>
                                            <asp:TextBox runat="server" ID="txtStartDate" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4 mb-2">
                                            <label class="form-label">End Date</label>
                                            <asp:TextBox runat="server" ID="txtEndDate" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4">
                                            <label class="form-label">Active</label>
                                            <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row mb-3 form-group">
                                        <div class="col-12">
                                            <label class="form-label">Title</label>
                                            <asp:TextBox runat="server" ID="txtTitle" CssClass="form-control" placeholder="Title ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mb-3 form-group">
                                        <div class="col-12">
                                            <label class="form-label">Message</label>
                                            <div id="summernote"></div>
                                        </div>
                                    </div>
                                    <div class="row mt-3" runat="server" id="divError">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgError"></span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClientClick="return setSummernoteContent();" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-4"></div>
        </section>
    </div>

    <asp:HiddenField runat="server" ID="fieldMessage" />
    <asp:HiddenField runat="server" ID="hfMessage" />

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#summernote').summernote({
                tabsize: 2,
                height: 350,

                callbacks: {
                    onChange: function (contents) {
                        $('#<%= hfMessage.ClientID %>').val(contents);
                    }
                }
            });

            $('#summernote').summernote('code', $('#<%= hfMessage.ClientID %>').val());
        });

        function setSummernoteContent() {
            var content = $('#summernote').summernote('code');
            $('#<%= fieldMessage.ClientID %>').val(content);
            return true;
        }
    </script>
</asp:Content>
