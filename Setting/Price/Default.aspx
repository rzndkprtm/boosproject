<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Price_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Master Price" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card-clickable {
            cursor: pointer;
            transition: transform 0.15s ease, box-shadow 0.15s ease;
            }
        
        .card-clickable:hover {
            transform: translateY(-3px);
            box-shadow: 0 4px 12px rgba(0,0,0,.15);
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
                <div class="card card-clickable" runat="server" id="divGroup">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Price Group</h6>
                                <h6 class="font-extrabold mb-0" runat="server"><%= GetSumData("PriceGroups") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divProductGroup">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Price Product Group</h6>
                                <h6 class="font-extrabold mb-0" runat="server"><%= GetSumData("PriceProductGroups") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divBase">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Price Base</h6>
                                <h6 class="font-extrabold mb-0" runat="server"><%= GetSumData("PriceBases") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divSurcharge">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Price Surcharge</h6>
                                <h6 class="font-extrabold mb-0" runat="server"><%= GetSumData("PriceSurcharges") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divPromo">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Price Promo</h6>
                                <h6 class="font-extrabold mb-0" runat="server"><%= GetSumData("Promos") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>