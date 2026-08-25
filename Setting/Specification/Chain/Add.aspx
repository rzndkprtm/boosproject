<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Specification_Chain_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Chain" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification">Specification</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification/blind">Blind</a></li>
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
                        <h4 class="card-title">Chain Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                        <label class="form-label">BOE ID</label>
                                        <asp:TextBox runat="server" ID="txtBoeId" CssClass="form-control" placeholder="BOE ID ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-8 form-group">
                                        <label class="form-label">Name</label>
                                        <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Design Name</label>
                                        <asp:ListBox runat="server" ID="lbDesign" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Sub Company</label>
                                        <asp:ListBox runat="server" ID="lbCompanyDetail" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-6 form-group">
                                        <label class="form-label">Chain Type</label>
                                        <asp:DropDownList runat="server" ID="ddlChainType" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Continuous" Text="Continuous"></asp:ListItem>
                                            <asp:ListItem Value="Non Continuous" Text="Non Continuous"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-6 form-group">
                                        <label class="form-label">Chain Length</label>
                                        <asp:DropDownList runat="server" ID="ddlChainLength" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Static" Text="Static"></asp:ListItem>
                                            <asp:ListItem Value="Flexible" Text="Flexible"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Description</label>
                                        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row" runat="server" id="divStatus">
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
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>