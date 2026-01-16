<%@ Page Title="Contact" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="Contact.aspx.vb" Inherits="Contact" MaintainScrollPositionOnPostback="true" Debug="true" %>

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

        

        <div class="container mt-4">
            <div class="mb-5">
                <h4 class="fw-semibold mb-3">IT</h4>
                <div class="row g-4">
                    <div class="col-12 col-md-6 col-lg-3">
                        <div class="card h-100 shadow-sm border-0">
                            <div class="card-body text-center">
                                <div class="mb-3">
                                    <img runat="server" src="~/Assets/images/avatars.png" class="rounded-circle" style="width:96px;height:96px;object-fit:cover;" />
                                </div>
                                
                                <h6 class="mb-1">Reza Andika Pratama</h6>
                                <p class="text-muted small mb-3">
                                    System & Application Support
                                </p>
                                
                                <div class="text-start small text-muted">
                                    <p class="mb-2">
                                        <strong>Email</strong><br />
                                        reza@bigblinds.co.id
                                    </p>
                                    <p class="mb-2">
                                        <strong>WhatsApp</strong><br />
                                        +62 852-1504-3355
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <!-- copy col-... untuk profile IT lain -->
                </div>
            </div>
            
            <!-- SALES TEAM -->
            <%--<div class="mb-5">
                <h4 class="fw-semibold mb-3">Sales</h4>
                <div class="row g-4">
                    <div class="col-12 col-md-6 col-lg-3">
                        <div class="card h-100 shadow-sm border-0">
                            <div class="card-body text-center">
                                <div class="mb-3">
                                    <img runat="server" src="~/Assets/images/avatars.png" class="rounded-circle" style="width:96px;height:96px;object-fit:cover;" />
                                </div>
                                
                                <h6 class="mb-1">Matt McCamey</h6>
                                <p class="text-muted small mb-3">
                                    National Sales Manager
                                </p>
                                
                                <div class="text-start small text-muted">
                                    <p class="mb-2">
                                        <strong>Email</strong><br />
                                        matt@jpmdirect.com.au
                                    </p>
                                    <p class="mb-0">
                                        <strong>Phone</strong><br />
                                         0417 705 109
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>--%>
        </div>

    </div>
</asp:Content>
