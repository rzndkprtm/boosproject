<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Search.aspx.vb" Inherits="Search" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Title="Global Search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted">Search across BOOS database.</p>
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
    <section class="section">
        <div class="card">
            <div class="card-body">
                <div class="row">
                    <div class="col-lg-8">
                        <div class="input-group">
                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" autocomplete="off" placeholder="Search anything..." />
                            <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                <i class="bi bi-search"></i> Search
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
                <div class="mt-3">
                    <asp:Label runat="server" ID="lblSearchInfo" CssClass="text-muted small"></asp:Label>
                    <asp:Label runat="server" ID="lblError" CssClass="text-danger small d-block"></asp:Label>
                </div>
            </div>
        </div>
        <asp:Panel runat="server" ID="pnlResults" Visible="false">
            <asp:Repeater runat="server" ID="rptCategories">
                <ItemTemplate>
                    <div class="card">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h5 class="mb-0">
                                <i class='<%# Eval("Icon") %> me-2'></i>
                                <%# Eval("Category") %>
                            </h5>
                            <span class="badge bg-primary">
                                <%# Eval("Count") %> Results
                            </span>
                        </div>
                        <div class="card-body">
                            <asp:Repeater runat="server" DataSource='<%# Eval("Items") %>'>
                                <ItemTemplate>
                                    <div class="border-bottom py-2">
                                        <a href='<%# Eval("Url") %>' class="text-decoration-none">
                                            <h6 class="text-primary mb-0"><%#: Eval("Title") %></h6>
                                        </a>
                                        <div class="text-muted small" style="white-space: pre-line; line-height: 1.3;"><%# Eval("Description") %></div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Panel runat="server" ID="pnlNoResults" Visible="false">
                <div class="card">
                    <div class="card-body text-center py-5">
                        <i class="bi bi-search fs-1 text-muted"></i>
                        <h5 class="mt-3">No Results Found</h5>
                        <p class="text-muted">We couldn't find anything matching your search.</p>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
    </section>
</asp:Content>