<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Specs_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Specs" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted">Product Information</p>
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
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Product Information</h4>
                    </div>
                    <div class="card-body text-center py-5">
                        <i class="bi bi-folder2-open" style="font-size: 3rem; color: #435ebe;"></i>
                        <h4 class="mt-3">Under Development</h4>
                        <p class="text-muted mt-3 mb-4">
                            Our Product Information section is currently under development.
                            <br />
                            In the meantime, you may access the shared folder using the button below.
                        </p>
                        <a href="https://onedrive.live.com/?id=%2Fpersonal%2Fae9316f9f664f948%2FDocuments%2FSPEED%20OneDrive%2Fno%20logo%20specs&listurl=%2Fpersonal%2Fae9316f9f664f948%2FDocuments&e=q6bhMl&migratedtospo=true&redeem=aHR0cHM6Ly8xZHJ2Lm1zL3UvcyFBa2o1WlBiNUZwT3VoemFHX2ZQQ0VrSWlqeW5kP2U9cTZiaE1s&ga=1" target="_blank" rel="noopener noreferrer" class="btn btn-primary">
                            <i class="bi bi-box-arrow-up-right me-2"></i>Access Shared Folder
                        </a>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>