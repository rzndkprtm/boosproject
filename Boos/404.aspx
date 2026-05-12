<%@ Page Language="VB" AutoEventWireup="false" CodeFile="404.aspx.vb" Inherits="Boos_404" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="404" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-content">
        <section class="row">
            <div class="col-12">
                <div class="card shadow-sm border-0">
                    <div class="card-body text-center py-5">
                        <div class="mb-4">
                            <i class="bi bi-exclamation-triangle" style="font-size: 80px; color: #dc3545;"></i>
                        </div>
                        <h1 class="fw-bold mb-2">404</h1>
                        <h3 class="fw-semibold mb-3">Page Not Found</h3>
                        <p class="text-muted mb-4" style="max-width: 700px; margin: auto;">The page you are looking for might have been removed, had its name changed, or is temporarily unavailable.</p>
                        <button type="button" class="btn btn-primary px-4" onclick="window.location.href='<%= ResolveUrl("~/") %>'">Back to Home</button>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>