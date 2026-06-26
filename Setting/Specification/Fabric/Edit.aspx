<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Edit.aspx.vb" Inherits="Setting_Specification_Fabric_Edit" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Edit Fabric Type" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification/fabric">Fabric</a></li>
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
                        <h4 class="card-title">Fabric Type Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-7 form-group">
                                            <label class="form-label">Name</label>
                                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-5 form-group">
                                            <label class="form-label">Type</label>
                                            <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Blockout" Text="Blockout"></asp:ListItem>
                                                <asp:ListItem Value="Light Filtering" Text="Light Filtering"></asp:ListItem>
                                                <asp:ListItem Value="Screen" Text="Screen"></asp:ListItem>
                                                <asp:ListItem Value="Sheer" Text="Sheer"></asp:ListItem>
                                                <asp:ListItem Value="Coated" Text="Coated"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Company Detail</label>
                                            <asp:ListBox runat="server" ID="lbCompany" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                        </div>
                                    </div>
                                    <asp:UpdatePanel runat="server" ID="upDesignTube" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <label class="form-label">Design Type</label>
                                                    <asp:ListBox runat="server" ID="lbDesign" CssClass="choices form-select multiple-remove" SelectionMode="Multiple" AutoPostBack="true" OnSelectedIndexChanged="lbDesign_SelectedIndexChanged"></asp:ListBox>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <label class="form-label">Tube Type</label>
                                                    <asp:ListBox runat="server" ID="lbTube" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <label class="form-label">Group</label>
                                            <asp:DropDownList runat="server" ID="ddlGroup" CssClass="form-select">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Group 1" Text="Group 1"></asp:ListItem>
                                                <asp:ListItem Value="Group 2" Text="Group 2"></asp:ListItem>
                                                <asp:ListItem Value="Group 3" Text="Group 3"></asp:ListItem>
                                                <asp:ListItem Value="Group 4" Text="Group 4"></asp:ListItem>
                                                <asp:ListItem Value="Group 1 (Express)" Text="Group 1 (Express)"></asp:ListItem>
                                                <asp:ListItem Value="Group 2 (Express)" Text="Group 2 (Express)"></asp:ListItem>
                                                <asp:ListItem Value="Opaque" Text="Opaque"></asp:ListItem>
                                                <asp:ListItem Value="Semi Opaque" Text="Semi Opaque"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <label class="form-label">No Rail Road</label>
                                            <asp:DropDownList runat="server" ID="ddlNoRailRoad" CssClass="form-select">
                                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row" runat="server" id="divError">
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
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-5"></div>
        </section>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>

    <script type="text/javascript">
        window.addEventListener("pageshow", function () {
            var loading = document.getElementById("loadingOverlay");
            if (loading) {
                loading.style.display = "none";
            }
        });
        function initChoices() {
            document.querySelectorAll("select.choices").forEach(function (el) {

                if (el.choices) {
                    el.choices.destroy();
                    el.choices = null;
                }

                new Choices(el, {
                    searchEnabled: true,
                    itemSelectText: '',
                    shouldSort: false,
                    removeItemButton: el.classList.contains("multiple-remove")
                });
            });
        }
        function initUpdatePanelLoading() {
            if (typeof (Sys) === "undefined") return;
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) {
                    loading.style.display = "block";
                }
            });
            prm.add_endRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) {
                    loading.style.display = "none";
                }
            });
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            initChoices();

        });
        if (typeof (Sys) !== "undefined") {
            Sys.Application.add_load(function () {
                initChoices();
            });
        }
    </script>
</asp:Content>