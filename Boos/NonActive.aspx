<%@ Page Language="VB" AutoEventWireup="false" CodeFile="NonActive.aspx.vb" Inherits="Boos_NonActive" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Non Active" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-content">
        <section class="row">
            <div class="col-12">
                <div class="card shadow-sm border-0">
                    <div class="card-body text-center py-5">
                        <div class="mb-4">
                            <i class="bi bi-person-x" style="font-size: 80px; color: #dc3545;"></i>
                        </div>
                        <h3 class="fw-bold mb-3">Account Inactive</h3>
                        <p class="text-muted mb-4" style="max-width: 700px; margin: auto;">Your account is currently inactive. Please contact the customer service to reactivate your account or check your account status.</p>
                        <button type="button" class="btn btn-primary px-4" onclick="window.location.href='<%= ResolveUrl("~/") %>'">Back to Home</button>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>
