<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_General_Newsletter_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Newsletter" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/newsletter">Newsletter</a></li>
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
                        <h4 class="card-title">Add</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-horizontal">
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Name</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-9 form-group">
                                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Type</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-3 form-group">
                                            <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Link" Text="Link"></asp:ListItem>
                                                <asp:ListItem Value="Image" Text="Image"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row" runat="server" id="divLink">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Link</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-9 form-group">
                                            <asp:TextBox runat="server" ID="txtLink" CssClass="form-control" placeholder="Url ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="row" runat="server" id="divFile">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>File</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-9 form-group">
                                            <asp:FileUpload runat="server" ID="fuFile" CssClass="form-control" />
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Description</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-9 form-group">
                                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ...." autocomplete="off" style="resize: none"></asp:TextBox>
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
</asp:Content>