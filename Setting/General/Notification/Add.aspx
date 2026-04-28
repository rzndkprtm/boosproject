<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_General_Notification_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Notification Add" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/general">notification</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-lg-8 col-md-12 col-sm-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Add Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-horizontal">
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Type</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-5 form-group">
                                            <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select" AutoPostBack="true"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Title</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-7 form-group">
                                            <asp:TextBox runat="server" ID="txtTitle" CssClass="form-control" placeholder="Title ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mb-3">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Message</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-9 form-group">
                                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtMessage" Height="200px" CssClass="form-control" placeholder="Message ...." autocomplete="off" style="resize: none"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Period</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-3 form-group">
                                            <asp:TextBox runat="server" ID="txtStartDate" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-3 form-group">
                                            <asp:TextBox runat="server" ID="txtEndDate" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Active</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-3 form-group">
                                            <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-4 col-md-12 col-sm-12"></div>
        </section>
    </div>
</asp:Content>
