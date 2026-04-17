<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Chat_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Chat" %>

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
                                            <div class="chat-item" onclick="loadChat('<%# Eval("Id") %>')">
                                                <%--<b><%# Eval("CustomerName") %></b><br />--%>
                                                <b>Reza</b><br />
                                                <%--<small><%# Eval("LastMessageDate") %></small>--%>
                                                <small>S</small>
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
