<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Ticket_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Ticket Detail" %>

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
        <section class="row mb-3">
            <div class="col-12">
                <div class="row mb-2" runat="server" id="divError">
                    <div class="col-12">
                        <div class="alert alert-danger">
                            <span runat="server" id="msgError"></span>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row mb-3">
            <div class="col-12 d-flex justify-content-end flex-wrap gap-2">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="New Ticket" />
                <asp:Button runat="server" ID="btnTopics" CssClass="btn btn-danger" Text="Set Topic" />
            </div>
        </section>
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-5">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                <div class="input-group">
                                    <span class="input-group-text">Search : </span>
                                    <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                    <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" />
                                </div>
                            </asp:Panel>
                        </div>
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12">
                                    <asp:Repeater ID="rptChatList" runat="server">
                                        <ItemTemplate>
                                            <div class="list-group-item list-group-item-action">
                                                <div class="d-flex w-100 justify-content-between">
                                                    <h5 class="mb-3">
                                                        <%# Eval("TicketCode") %> - <%# Eval("TopicName") %>
                                                    </h5>
                                                    <small><%# Eval("CreatedDate", "{0:dd MMM yyyy HH:mm}") %></small>
                                                </div>
                                                <p runat="server" id="pMessage" class="mb-3">Last Message</p>
                                                <small>Created by <b><%# Eval("LoginName") %></b></small>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-7">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header"></div>
                        <div class="card-body"></div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>