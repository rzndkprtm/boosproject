<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Edit.aspx.vb" Inherits="Setting_Price_Promo_Edit" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Edit Price Promo" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price">Price</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/promo">Promo</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-6">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Promo Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <label class="form-label">Company</label>
                                            <asp:DropDownList runat="server" ID="ddlCompany" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <label class="form-label">Type</label>
                                            <asp:DropDownList runat="server" ID="ddlType" CssClass="choices form-select">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Sell" Text="Promo Sell"></asp:ListItem>
                                                <asp:ListItem Value="Buy" Text="Promo Buy"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Name</label>
                                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Promo Name ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <label class="form-label">Start Date</label>
                                            <asp:TextBox runat="server" ID="txtStartDate" TextMode="Date" CssClass="form-control" placeholder="Start Date ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <label class="form-label">End Date</label>
                                            <asp:TextBox runat="server" ID="txtEndDate" TextMode="Date" CssClass="form-control" placeholder="Start Date ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Description</label>
                                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-4 form-group">
                                            <label class="form-label">Active</label>
                                            <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
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
                    </div>
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-6">
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

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblReturnPage"></asp:Label>
    </div>

    <script type="text/javascript">
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>