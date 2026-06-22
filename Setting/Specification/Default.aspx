<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Specification_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Specification" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .dashboard-wrapper { display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; width: 100%; }
        .dashboard-card { position: relative; display: block; background: #fff; border-radius: 16px; padding: 24px; border: 1px solid #e5e5e5; box-shadow: 0 2px 8px rgba(0,0,0,0.06); transition: 0.2s ease; cursor: pointer; text-decoration: none; overflow: hidden; }
        .dashboard-card:hover { transform: translateY(-3px); box-shadow: 0 6px 18px rgba(0,0,0,0.12); }
        .dashboard-number { font-size: 38px; font-weight: 700; color: #222; line-height: 1; margin-bottom: 14px; }
        .dashboard-title { font-size: 20px; font-weight: 600; color: #222; margin-bottom: 6px; }
        .dashboard-desc { font-size: 13px; color: #777; line-height: 1.5; }
        @media (max-width: 1200px) { .dashboard-wrapper { grid-template-columns: repeat(2, 1fr); } }
        @media (max-width: 768px) { .dashboard-wrapper { grid-template-columns: 1fr; } }
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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
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
                <a href="/setting/specification/designtype" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("Designs") %></div>
                    <div class="dashboard-title">Design Type</div>
                    <div class="dashboard-desc">Description</div>
                </a>
                <a href="/setting/specification/blindtype" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("Blinds") %></div>
                    <div class="dashboard-title">Blind Type</div>
                    <div class="dashboard-desc">Description</div>
                </a>
                <a href="/setting/specification/product" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("Products") %></div>
                    <div class="dashboard-title">Product</div>
                    <div class="dashboard-desc">Description</div>
                </a>
                <a href="/setting/specification/fabric" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("Fabrics") %></div>
                    <div class="dashboard-title">Fabric Type</div>
                    <div class="dashboard-desc">Description</div>
                </a>
                <a href="/setting/specification/chain" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("Chains") %></div>
                    <div class="dashboard-title">Chain</div>
                    <div class="dashboard-desc">Description</div>
                </a>
                <a href="/setting/specification/remote" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("Remotes") %></div>
                    <div class="dashboard-title">Remote</div>
                    <div class="dashboard-desc">Description</div>
                </a>
                <a href="/setting/specification/bottom" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("Bottoms") %></div>
                    <div class="dashboard-title">Bottom Type</div>
                    <div class="dashboard-desc">Description</div>
                </a>
                <a href="/setting/specification/mounting" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("Mountings") %></div>
                    <div class="dashboard-title">Mounting</div>
                    <div class="dashboard-desc">Description</div>
                </a>
            </div>
        </section>
        <div runat="server" id="divHr"><hr /></div>
        <section class="row mt-2" runat="server" id="divAdditional">
            <div class="dashboard-wrapper">
                <a href="/setting/specification/tubetype" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("ProductTubes") %></div>
                    <div class="dashboard-title">Tube Type</div>
                    <div class="dashboard-desc">Description</div>
                </a>
                <a href="/setting/specification/controltype" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("ProductControls") %></div>
                    <div class="dashboard-title">Control Type</div>
                    <div class="dashboard-desc">Description</div>
                </a>
                <a href="/setting/specification/colourtype" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("ProductColours") %></div>
                    <div class="dashboard-title">Colour Type</div>
                    <div class="dashboard-desc">Description</div>
                </a>
            </div>
        </section>
    </div>
</asp:Content>