<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Guide.aspx.vb" Inherits="Guide" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Title="Guide" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .tutorial-card { border: 1px solid #e9ecef; border-radius: 10px; transition: all .2s ease; }
        .tutorial-card:hover { transform: translateY(-3px); box-shadow: 0 5px 18px rgba(0,0,0,.08); }
        .tutorial-icon { width: 42px; height: 42px; min-width: 42px; display: flex; align-items: center; justify-content: center; background: #eef2ff; color: #435ebe; border-radius: 9px; font-size: 21px; }
        .tutorial-card .card-title { font-weight: 600; color: #25396f; line-height: 1.5; }
        .tutorial-description { font-size: 14px; line-height: 1.7; overflow-wrap: anywhere; }
        .tutorial-description img { max-width: 100%; height: auto; }
        .tutorial-description iframe { max-width: 100%; }
        .tutorial-description ul, .tutorial-description ol { padding-left: 20px; }
        .tutorial-card .btn { border-radius: 6px; }
        .tutorial-company { font-size: 12px; color: #435ebe; background: #eef2ff; padding: 4px 9px; border-radius: 5px; display: inline-block; margin-top: 6px; }
    </style>
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted">Browse guides and learn how to use the system.</p>
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
        <asp:Panel ID="pnlError" runat="server" Visible="false">
            <div class="alert alert-danger">
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </div>
        </asp:Panel>
        <div class="card mb-4">
            <div class="card-body">
                <div class="row g-2">
                    <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                        <div class="input-group">
                            <span class="input-group-text">Search : </span>
                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" autocomplete="off"></asp:TextBox>
                            <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
            <div class="card">
                <div class="card-body text-center py-5">
                    <i class="bi bi-journal-bookmark fs-1 text-muted"></i>
                    <h5 class="mt-3">No Tutorials Found</h5>
                    <p class="text-muted mb-0">No tutorials match your search.</p>
                </div>
            </div>
        </asp:Panel>
        <div class="row">
            <asp:Repeater ID="rptTutorial" runat="server">
                <ItemTemplate>
                    <div class="col-12 col-md-6 col-xl-4 mb-4">
                        <div class="card h-100 tutorial-card">
                            <div class="card-body d-flex flex-column">
                                <div class="d-flex align-items-start mb-3">
                                    <div class="tutorial-icon me-3">
                                        <i class="bi bi-journal-text"></i>
                                    </div>
                                    <div class="flex-grow-1">
                                        <h5 class="card-title mb-0">
                                            <%# Server.HtmlEncode(Convert.ToString(Eval("Title"))) %>
                                        </h5>
                                        <span class="tutorial-company">
                                            <i class="bi bi-building me-1"></i>
                                            <%# Server.HtmlEncode(Convert.ToString(Eval("CompanyName"))) %>
                                        </span>
                                    </div>
                                </div>
                                <div class="tutorial-description text-muted mb-4">
                                    <asp:Literal ID="litDescription" runat="server" Mode="PassThrough" Text='<%# Eval("Description") %>'>
                                    </asp:Literal>
                                </div>
                                <div class="mt-auto d-flex flex-wrap gap-2">
                                    <asp:HyperLink ID="lnkFile" runat="server" NavigateUrl='<%# GetFileUrl(Eval("File")) %>' Visible='<%# Not String.IsNullOrWhiteSpace(Convert.ToString(Eval("File"))) %>' Target="_blank"  rel="noopener noreferrer" CssClass="btn btn-outline-primary btn-sm">
                                        <i class="bi bi-file-earmark-text me-1"></i> View File
                                    </asp:HyperLink>
                                    <asp:HyperLink ID="lnkVideo" runat="server" NavigateUrl='<%# GetSafeUrl(Eval("Video")) %>' Visible='<%# IsValidUrl(Eval("Video")) %>' Target="_blank" rel="noopener noreferrer" CssClass="btn btn-primary btn-sm">
                                        <i class="bi bi-play-circle me-1"></i>Watch Video
                                    </asp:HyperLink>
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </section>
</asp:Content>