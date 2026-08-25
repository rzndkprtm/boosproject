<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Edit.aspx.vb" Inherits="Setting_General_Company_Edit" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Edit Company" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/general">General</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-7 mb-2">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Company Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-7 form-group">
                                        <label class="form-label">Name</label>
                                        <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-5 form-group">
                                        <label class="form-label">Alias</label>
                                        <asp:TextBox runat="server" ID="txtAlias" CssClass="form-control" placeholder="Alias ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Attention</label>
                                        <asp:TextBox runat="server" ID="txtAttention" CssClass="form-control" placeholder="Attention ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Address</label>
                                        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtAddress" Height="100px" CssClass="form-control" placeholder="Address ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Phone</label>
                                        <asp:TextBox runat="server" ID="txtPhone" CssClass="form-control" placeholder="Phone ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Email</label>
                                        <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Description</label>
                                        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-3 form-group">
                                        <label class="form-label">Status</label>
                                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select">
                                            <asp:ListItem Value="Active" Text="Active"></asp:ListItem>
                                            <asp:ListItem Value="Inactive" Text="Inactive"></asp:ListItem>
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
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-5 mb-2"></div>
        </section>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblReturnPage"></asp:Label>
    </div>
</asp:Content>
