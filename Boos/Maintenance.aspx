<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Maintenance.aspx.vb" Inherits="Boos_Maintenance" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Maintenance" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-content">
        <section class="row">
            <div class="col-12">
                <div class="card shadow-sm border-0">
                    <div class="card-body text-center py-5">
                        <div class="mb-4">
                            <i class="bi bi-tools" style="font-size: 80px; color: #f39c12;"></i>
                        </div>
                        <h2 class="fw-bold mb-3">Web Under Maintenance</h2>
                        <p class="text-muted mb-4" style="max-width: 700px; margin: auto;">We are currently performing scheduled maintenance to improve system performance and reliability.
                            This page is temporarily unavailable. Please check back again later.</p>
                        <div class="alert alert-warning d-inline-block px-4 py-3 mb-4">
                            <strong>Notice:</strong> Some features may not be accessible during the maintenance period.
                        </div>
                        <div>
                            <button type="button" class="btn btn-primary px-4" onclick="window.location.href='<%= ResolveUrl("~/") %>'">Try Again</button>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>