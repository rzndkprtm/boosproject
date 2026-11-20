<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Contact.aspx.vb" Inherits="Contact" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <h3>Contact</h3>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <div class="text-center py-5">
                            <i class="bi bi-envelope-paper display-1 text-primary"></i>
                            <h3 class="mt-4 mb-3">Get in Touch</h3>
                            <p class="text-muted mb-0">For enquiries or assistance, please contact the relevant department.</p>
                        </div>
                        <div class="row g-4 mt-2">
                            <div class="col-md-6 col-lg-6">
                                <div class="border rounded p-4 h-100">
                                    <div class="d-flex align-items-center mb-3">
                                        <i class="bi bi-laptop fs-3 text-primary me-3"></i>
                                        <h5 class="mb-0">IT</h5>
                                    </div>
                                    <p class="text-muted mb-0">Technical support, system access, and application assistance.</p>
                                </div>
                            </div>
                            <div class="col-md-6 col-lg-6">
                                <div class="border rounded p-4 h-100">
                                    <div class="d-flex align-items-center mb-3">
                                        <i class="bi bi-bar-chart fs-3 text-primary me-3"></i>
                                        <h5 class="mb-0">Sales</h5>
                                    </div>
                                    <p class="text-muted mb-0">Sales enquiries, customer accounts, and sales-related assistance.</p>
                                </div>
                            </div>
                            <div class="col-md-6 col-lg-6">
                                <div class="border rounded p-4 h-100">
                                    <div class="d-flex align-items-center mb-3">
                                        <i class="bi bi-headset fs-3 text-primary me-3"></i>
                                        <h5 class="mb-0">Customer Service</h5>
                                    </div>
                                    <p class="text-muted mb-0">Customer enquiries, order updates, and general assistance.</p>
                                </div>
                            </div>
                            <div class="col-md-6 col-lg-6">
                                <div class="border rounded p-4 h-100">
                                    <div class="d-flex align-items-center mb-3">
                                        <i class="bi bi-calculator fs-3 text-primary me-3"></i>
                                        <h5 class="mb-0">Account</h5>
                                    </div>
                                    <p class="text-muted mb-0">Invoicing, payments, and account-related enquiries.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>