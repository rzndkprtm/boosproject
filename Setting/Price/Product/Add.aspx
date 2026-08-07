<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Price_Product_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Price Product Group" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .format-guide { background-color: #f8f9fa; border: 1px solid #dee2e6; border-radius: 6px; }
        .format-item { padding: 5px 0; }
        .format-item:not(:last-child) { border-bottom: 1px dashed #dee2e6; }
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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price">Price</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/product">Product Group</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-7">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Product Group Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-5 mb-2 form-group">
                                                <label class="form-label">Design Type</label>
                                                <asp:DropDownList runat="server" ID="ddlDesign" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesign_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-7 mb-2 form-group">
                                                <label class="form-label">Price Group</label>
                                                <asp:ListBox runat="server" ID="lbPriceGroup" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Name</label>
                                                <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="mt-2 mb-2">
                                            <label class="form-label fw-bold">Format Guide :</label>
                                            <div class="format-guide p-2 rounded">
                                                <asp:GridView runat="server" ID="gvList" AutoGenerateColumns="false" ShowHeader="false" GridLines="None" CssClass="w-100">
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <div class="format-item d-flex">
                                                                    <div class="me-2 text-primary">
                                                                        <i class="bi bi-info-circle-fill"></i>
                                                                    </div>
                                                                    <div>
                                                                        <div class="small text-muted"><%# Eval("Name") %></div>
                                                                        <div class="fw-semibold"><%# Eval("Format") %></div>
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Description</label>
                                        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" CssClass="form-control" Height="100px" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                        <label class="form-label">Status</label>
                                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select">
                                            <asp:ListItem Value="Active" Text="Active"></asp:ListItem>
                                            <asp:ListItem Value="Inactive" Text="Inactive"></asp:ListItem>
                                        </asp:DropDownList>
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
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-5">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title text-center">Information</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body"></div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <script type="text/javascript">
        window.addEventListener("pageshow", function () {
            var loading = document.getElementById("loadingOverlay");
            if (loading) loading.style.display = "none";
        });
        function initUpdatePanelLoading() {
            if (typeof Sys === "undefined") return;
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "block";
            });
            prm.add_endRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "none";
                initChoices();
            });
        }
        function initChoices() {
            document.querySelectorAll("select.choices").forEach(function (el) {

                if (el.choices) {
                    el.choices.destroy();
                }

                var isMultiple = el.multiple;

                el.choices = new Choices(el, {
                    searchEnabled: true,
                    itemSelectText: '',
                    shouldSort: false,
                    removeItemButton: isMultiple
                });
            });
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            initChoices();
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>