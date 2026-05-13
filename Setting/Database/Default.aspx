<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Database_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Master Database" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card-clickable { cursor: pointer; transition: transform 0.15s ease, box-shadow 0.15s ease; }        
        .card-clickable:hover { transform: translateY(-3px); box-shadow: 0 4px 12px rgba(0,0,0,.15); }
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
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divTable">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Table</h6>
                                <h6 class="font-extrabold mb-0" runat="server"></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divView">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">View</h6>
                                <h6 class="font-extrabold mb-0" runat="server"></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divFunction">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Function</h6>
                                <h6 class="font-extrabold mb-0" runat="server"></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divQuery">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Query</h6>
                                <h6 class="font-extrabold mb-0" runat="server"></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>