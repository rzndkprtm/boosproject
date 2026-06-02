<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Specification_Fabric_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Fabric Type" %>

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
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-4 form-group">
                                            <label class="form-label">Status</label>
                                            <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="In Stock" Text="In Stock"></asp:ListItem>
                                                <asp:ListItem Value="Limited Stock" Text="Limited Stock"></asp:ListItem>
                                                <asp:ListItem Value="Out of Stock" Text="Out of Stock"></asp:ListItem>
                                                <asp:ListItem Value="Discontinued" Text="Discontinued"></asp:ListItem>
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

    <script>
        function rebindChoices() {
            document.querySelectorAll("select.choices").forEach(function (el) {
                if (el.dataset.choice === "active") return;

                new Choices(el, {
                    removeItemButton: true
                });
            });
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            rebindChoices();
        });
    </script>
</asp:Content>