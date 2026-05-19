<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Setting" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .dashboard-wrapper { display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; width: 100%; }
        .dashboard-card { display: block; background: #fff; border-radius: 14px; padding: 22px; border: 1px solid #e5e5e5; box-shadow: 0 2px 8px rgba(0,0,0,0.06); transition: 0.2s ease; cursor: pointer; text-decoration: none; }
        .dashboard-card:hover { transform: translateY(-3px); box-shadow: 0 5px 15px rgba(0,0,0,0.12); }
        .dashboard-title { font-size: 20px; font-weight: 600; color: #222; margin-bottom: 8px; }
        .dashboard-desc { font-size: 13px; color: #777; line-height: 1.5; }
        @media (max-width: 1200px) {
            .dashboard-wrapper { grid-template-columns: repeat(2, 1fr); }
        }
        @media (max-width: 768px) {
            .dashboard-wrapper { grid-template-columns: 1fr; }
        }
    </style>

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
        <section class="row">
            <div class="dashboard-wrapper">
                <a href="/setting/general" class="dashboard-card">
                    <div class="dashboard-title">General</div>
                    <div class="dashboard-desc">
                        Company, Mailing, Login Access, Newsletter, Tutorial, Action Access
                    </div>
                </a>
                
                <a href="/setting/customer" class="dashboard-card">
                    <div class="dashboard-title">Customer</div>
                    <div class="dashboard-desc">
                        Customer List
                    </div>
                </a>
                <a href="/setting/specification" class="dashboard-card">
                    <div class="dashboard-title">Specification</div>
                    <div class="dashboard-desc">
                        Design Type, Blind Type, Product, Fabric, Chain, Remote, Bottom Rail, Mounting
                    </div>
                </a>
                <a href="/setting/price" class="dashboard-card">
                    <div class="dashboard-title">Price</div>
                    <div class="dashboard-desc">
                        Price Group, Price Product Group, Price Base, Price Surcharge, Price Promo
                    </div>
                </a>
                <a href="/setting/database" class="dashboard-card">
                    <div class="dashboard-title">Database</div>
                    <div class="dashboard-desc">
                        Table, View, Function, Query
                    </div>
                </a>
                <a href="/setting/online" class="dashboard-card">
                    <div class="dashboard-title">Online User</div>
                    <div class="dashboard-desc">
                        List User
                    </div>
                </a>
                <a href="/setting/xero" class="dashboard-card">
                    <div class="dashboard-title">Xero</div>
                    <div class="dashboard-desc">
                        List, Create, Update, Delete
                    </div>
                </a>
                <a href="/setting/notification" class="dashboard-card">
                    <div class="dashboard-title">Notification</div>
                    <div class="dashboard-desc">
                        List, Create, Update, Delete
                    </div>
                </a>
                <a href="/setting/log" class="dashboard-card">
                    <div class="dashboard-title">Log</div>
                    <div class="dashboard-desc">
                        History
                    </div>
                </a>
            </div>
        </section>
    </div>
</asp:Content>