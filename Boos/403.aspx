<%@ Page Language="VB" AutoEventWireup="false" CodeFile="403.aspx.vb" Inherits="Boos_403" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="403" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-content">
        <section class="row">
            <div class="col-12">
                <div class="card shadow-sm border-0">
                    <div class="card-body text-center py-5">
                        <div class="mb-4">
                            <i class="bi bi-shield-lock" style="font-size: 80px; color: #dc3545;"></i>
                        </div>
                        <h1 class="fw-bold mb-2">403</h1>
                        <h3 class="fw-semibold mb-3">Access Forbidden</h3>
                        <p class="text-muted mb-4" style="max-width: 700px; margin: auto;">You don’t have permission to access this page or resource.
                            Please contact your administrator if you believe this is a mistake.</p>
                        <button type="button" class="btn btn-primary px-4" onclick="window.location.href='<%= ResolveUrl("~/") %>'">Back to Home</button>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>