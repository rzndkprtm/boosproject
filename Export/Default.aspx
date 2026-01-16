<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Export_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Export Order" %>

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
                        <h4 class="card-title">Sort Data</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-horizontal">
                                <div class="form-body">
                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Company</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-5 form-group">
                                            <asp:DropDownList runat="server" ID="ddlOrderCompany" CssClass="form-select"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <asp:Label runat="server" Text="Order Status"></asp:Label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4 form-group">
                                            <asp:DropDownList runat="server" ID="ddlOrderStatus" CssClass="form-select">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Unsubmitted" Text="Unsubmitted"></asp:ListItem>
                                                <asp:ListItem Value="New Order" Text="New Order"></asp:ListItem>
                                                <asp:ListItem Value="Waiting Proforma" Text="Waiting Proforma"></asp:ListItem>
                                                <asp:ListItem Value="Proforma Sent" Text="Proforma Sent"></asp:ListItem>
                                                <asp:ListItem Value="Payment Received" Text="Payment Received"></asp:ListItem>
                                                <asp:ListItem Value="In Production" Text="In Production"></asp:ListItem>
                                                <asp:ListItem Value="On Hold" Text="On Hold"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <asp:Label runat="server" Text="Order Type"></asp:Label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-9 form-group">
                                            <asp:DropDownList runat="server" ID="ddlOrderType" CssClass="form-select">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="header" Text="Order Header"></asp:ListItem>
                                                <asp:ListItem Value="detail" Text="Order Detail"></asp:ListItem>
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
        </section>
    </div>
</asp:Content>
