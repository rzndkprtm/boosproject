<%@ Page Title="About" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="About.aspx.vb" Inherits="About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .about-page { color: #344054; padding-bottom: 30px; }
        .about-hero { position: relative; overflow: hidden; padding: 55px 40px; border-radius: 16px; background: linear-gradient(135deg, #172b4d 0%, #244d75 100%); color: #ffffff; margin-bottom: 28px; }
        .about-hero::after { content: ""; position: absolute; width: 280px; height: 280px; right: -70px; top: -110px; border: 45px solid rgba(255, 255, 255, 0.06); border-radius: 50%; pointer-events: none; }
        .about-hero-content { position: relative; z-index: 1; max-width: 760px; }
        .about-label { display: inline-block; padding: 7px 14px; margin-bottom: 18px; border: 1px solid rgba(255, 255, 255, 0.25); border-radius: 30px; background: rgba(255, 255, 255, 0.08); color: #e3edf8; font-size: 12px; font-weight: 700; letter-spacing: 1.5px; text-transform: uppercase; }
        .about-hero h1 { margin: 0 0 18px; color: #ffffff; font-size: clamp(30px, 4vw, 44px); font-weight: 800; line-height: 1.2; }
        .about-hero p { max-width: 680px; margin: 0; color: #dce7f3; font-size: 16px; line-height: 1.9; }
        .about-section { margin-bottom: 28px; }
        .about-card { height: 100%; padding: 28px; border: 1px solid #eaecf0; border-radius: 14px; background: #ffffff; box-shadow: 0 3px 12px rgba(16, 24, 40, 0.035); }
        .about-section-title { margin-bottom: 14px; color: #172b4d; font-size: 23px; font-weight: 750; line-height: 1.4; }
        .about-section-subtitle { margin-bottom: 22px; color: #667085; font-size: 14px; line-height: 1.8; }
        .about-text { margin-bottom: 14px; color: #667085; font-size: 14px; line-height: 1.95; }
        .about-text:last-child { margin-bottom: 0; }
        .about-highlight { padding: 25px; border-left: 4px solid #356fa3; border-radius: 8px; background: #f3f7fb; }
        .about-highlight p { margin: 0; color: #344054; font-size: 15px; font-weight: 500; line-height: 1.9; }
        .about-icon { display: flex; align-items: center; justify-content: center; width: 48px; height: 48px; margin-bottom: 18px; border-radius: 12px; background: #edf4fb; color: #285b89; font-size: 22px; }
        .about-feature-card { height: 100%; padding: 25px; border: 1px solid #eaecf0; border-radius: 14px; background: #ffffff; transition: transform 0.2s ease, box-shadow 0.2s ease; }
        .about-feature-card:hover { transform: translateY(-3px); box-shadow: 0 8px 22px rgba(16, 24, 40, 0.07); }
        .about-feature-card h5 { margin-bottom: 10px; color: #172b4d; font-size: 16px; font-weight: 750; }
        .about-feature-card p { margin: 0; color: #667085; font-size: 13px; line-height: 1.85; }
        .about-product-list { display: flex; flex-wrap: wrap; gap: 10px; margin-top: 20px; }
        .about-product-item { display: inline-flex; align-items: center; gap: 8px; padding: 10px 14px; border: 1px solid #e4e7ec; border-radius: 8px; background: #ffffff; color: #344054; font-size: 13px; font-weight: 600; }
        .about-product-item i { color: #356fa3; font-size: 15px; }
        .about-delivery-card { display: flex; align-items: flex-start; gap: 20px; padding: 28px; border-radius: 14px; background: #f3f7fb; }
        .about-delivery-number { flex-shrink: 0; color: #285b89; font-size: 42px; font-weight: 800; line-height: 1.1; }
        .about-delivery-number span { display: block; margin-top: 5px; color: #667085; font-size: 12px; font-weight: 700; letter-spacing: 1px; text-transform: uppercase; }
        .about-delivery-content h4 { margin-bottom: 10px; color: #172b4d; font-size: 19px; font-weight: 750; }
        .about-delivery-content p { margin: 0; color: #667085; font-size: 14px; line-height: 1.9; }
        .about-commitment { position: relative; overflow: hidden; padding: 38px; border-radius: 16px; background: #172b4d; color: #ffffff; }
        .about-commitment h3 { margin-bottom: 18px; color: #ffffff; font-size: 25px; font-weight: 800; }
        .about-commitment p { margin-bottom: 15px; color: #dce7f3; font-size: 14px; line-height: 1.95; }
        .about-commitment .about-commitment-quote { margin-top: 22px; margin-bottom: 0; padding-top: 20px; border-top: 1px solid rgba(255, 255, 255, 0.18); color: #ffffff; font-size: 17px; font-weight: 700; line-height: 1.8; }
        @media (max-width: 767.98px) {
            .about-hero { padding: 35px 24px; }
            .about-card, .about-feature-card { padding: 22px; }
            .about-section-title { font-size: 21px; }
            .about-delivery-card { flex-direction: column; gap: 15px; padding: 24px; }
            .about-commitment { padding: 28px 24px; }
        }
    </style>

    <div class="page-heading">
        <h3>About</h3>
    </div>

    <div class="page-content about-page">
        <section class="about-hero">
            <div class="about-hero-content">
                <span class="about-label">About Our Company</span>
                <h1>Get to Know JPM Direct</h1>
                <p>
                    Your trusted partner in custom window furnishings.
                    Combining manufacturing expertise, quality craftsmanship
                    and personal service to deliver exceptional products
                    at competitive factory-direct prices.
                </p>
            </div>
        </section>
        <section class="about-section">
            <div class="row g-4">
                <div class="col-12 col-lg-7">
                    <div class="about-card">
                        <div class="about-icon">
                            <i class="bi bi-building"></i>
                        </div>
                        <h4 class="about-section-title">Who We Are</h4>
                        <p class="about-text">
                            <strong>JPM Direct Pty Ltd</strong> was established
                            in February 2010 and has quickly established itself
                            as a leading wholesaler in Australia's custom window
                            furnishings industry.
                        </p>
                        <p class="about-text">
                            Supported by the expertise and manufacturing
                            capabilities of our Indonesian factory, we offer
                            an extensive range of window covering products
                            at highly competitive prices. Our factory is the
                            largest producer of custom-made blinds in Indonesia.
                        </p>
                        <p class="about-text">
                            Our continued growth reflects our commitment to
                            delivering quality products, dependable service
                            and practical solutions that help our customers
                            succeed in a competitive market.
                        </p>
                    </div>
                </div>
                <div class="col-12 col-lg-5">
                    <div class="about-card">
                        <div class="about-icon">
                            <i class="bi bi-people"></i>
                        </div>
                        <h4 class="about-section-title">Our People</h4>
                        <p class="about-text">
                            Our professional sales team brings together more
                            than 100 years of combined experience in the
                            window furnishings industry.
                        </p>
                        <p class="about-text">
                            This collective knowledge enables us to provide
                            technical product advice, market insight and the
                            expertise needed to deliver what customers want.
                        </p>
                        <p class="about-text">
                            We believe our people, their experience, drive
                            and dedication are fundamental to our success.
                        </p>
                    </div>
                </div>
            </div>
        </section>
        <section class="about-section">
            <div class="mb-4">
                <h4 class="about-section-title">What Makes Us Different</h4>
                <p class="about-section-subtitle">
                    We combine manufacturing strength, industry knowledge
                    and customer-focused service to support our retail partners.
                </p>
            </div>
            <div class="row g-4">
                <div class="col-12 col-md-6 col-xl-3">
                    <div class="about-feature-card">
                        <div class="about-icon">
                            <i class="bi bi-gear-wide-connected"></i>
                        </div>
                        <h5>Manufacturing Expertise</h5>
                        <p>
                            Backed by extensive manufacturing capabilities
                            in Indonesia, we produce custom-made window
                            furnishings to meet a wide range of requirements.
                        </p>
                    </div>
                </div>
                <div class="col-12 col-md-6 col-xl-3">
                    <div class="about-feature-card">
                        <div class="about-icon">
                            <i class="bi bi-award"></i>
                        </div>
                        <h5>Quality Craftsmanship</h5>
                        <p>
                            We use carefully selected materials, quality
                            components and skilled craftsmanship to deliver
                            products built to high standards.
                        </p>
                    </div>
                </div>
                <div class="col-12 col-md-6 col-xl-3">
                    <div class="about-feature-card">
                        <div class="about-icon">
                            <i class="bi bi-currency-dollar"></i>
                        </div>
                        <h5>Competitive Pricing</h5>
                        <p>
                            Our factory-direct model helps retailers offer
                            quality window furnishings without the burden
                            of maintaining their own manufacturing facilities.
                        </p>
                    </div>
                </div>
                <div class="col-12 col-md-6 col-xl-3">
                    <div class="about-feature-card">
                        <div class="about-icon">
                            <i class="bi bi-headset"></i>
                        </div>
                        <h5>Personal Service</h5>
                        <p>
                            We support our customers with product knowledge,
                            online ordering, training resources and
                            dependable customer service.
                        </p>
                    </div>
                </div>
            </div>
        </section>
        <section class="about-section">
            <div class="about-card">
                <div class="about-icon">
                    <i class="bi bi-window"></i>
                </div>
                <h4 class="about-section-title">Our Products</h4>
                <p class="about-text">
                    We specialise in custom-made window furnishings,
                    offering a broad selection of products to suit
                    different interior styles, functional needs and budgets.
                </p>
                <div class="about-product-list">
                    <span class="about-product-item">
                        <i class="bi bi-check-circle-fill"></i>
                        Timber Venetian Blinds
                    </span>
                    <span class="about-product-item">
                        <i class="bi bi-check-circle-fill"></i>
                        Econo Wood Blinds
                    </span>
                    <span class="about-product-item">
                        <i class="bi bi-check-circle-fill"></i>
                        Aluminium Venetian Blinds
                    </span>
                    <span class="about-product-item">
                        <i class="bi bi-check-circle-fill"></i>
                        Roller Blinds
                    </span>
                    <span class="about-product-item">
                        <i class="bi bi-check-circle-fill"></i>
                        Roman Blinds
                    </span>
                    <span class="about-product-item">
                        <i class="bi bi-check-circle-fill"></i>
                        Panel Glides
                    </span>
                    <span class="about-product-item">
                        <i class="bi bi-check-circle-fill"></i>
                        Curtains
                    </span>
                    <span class="about-product-item">
                        <i class="bi bi-check-circle-fill"></i>
                        Designer Shades
                    </span>
                </div>
            </div>
        </section>
        <section class="about-section">
            <div class="row g-4">
                <div class="col-12 col-lg-6">
                    <div class="about-card">
                        <div class="about-icon">
                            <i class="bi bi-tools"></i>
                        </div>
                        <h4 class="about-section-title">Quality & Craftsmanship</h4>
                        <p class="about-text">
                            Our objective is to produce window furnishings
                            to the highest available standards. We begin
                            with carefully selected materials, including
                            resilient hardwoods, tempered aluminium,
                            quality fabrics, furniture-grade finishes
                            and durable components.
                        </p>
                        <p class="about-text">
                            Our skilled craftsmen then cut, shape and assemble
                            each blind and shade to order. Every product
                            undergoes a stringent inspection by our quality
                            control department before dispatch.
                        </p>
                    </div>
                </div>
                <div class="col-12 col-lg-6">
                    <div class="about-card">
                        <div class="about-icon">
                            <i class="bi bi-box-seam"></i>
                        </div>
                        <h4 class="about-section-title">A Better Way to Do Business</h4>
                        <p class="about-text">
                            JPM Direct provides a practical solution for
                            retailers who prefer to focus on sales rather
                            than manufacturing.
                        </p>
                        <p class="about-text">
                            By partnering with us, retailers can reduce
                            the need to manage production, stock and
                            manufacturing overheads, while benefiting
                            from our manufacturing capabilities and
                            industry experience.
                        </p>
                        <p class="about-text">
                            We take care of the production process so our
                            partners can concentrate on growing their business
                            and serving their customers.
                        </p>
                    </div>
                </div>
            </div>
        </section>
        <section class="about-section">
            <div class="about-delivery-card">
                <div class="about-delivery-number">7 <span>Days</span></div>
                <div class="about-delivery-content">
                    <h4>Efficient Manufacturing & Delivery</h4>
                    <p>
                        All orders are manufactured and air-freighted to
                        our warehouses in Australia within 7 days from
                        the date of order. This efficient process helps
                        us provide competitive delivery times and reliable
                        service to our retail partners.
                    </p>
                </div>
            </div>
        </section>
        <section class="about-commitment">
            <div class="row">
                <div class="col-12 col-lg-10">
                    <span class="about-label">Our Commitment</span>
                    <h3>Growing Together, Delivering Excellence</h3>
                    <p>
                        Our business has grown through the quality of our
                        products and the dedication of our people. As we
                        continue to evolve, we remain committed to providing
                        superior window furnishings at factory-direct prices,
                        supported by the service our customers deserve.
                    </p>
                    <p>
                        We continue to improve our offering through online
                        ordering, easy-to-follow measurement and installation
                        instructions, user-friendly product training modules
                        and innovative new products.
                    </p>
                    <p>
                        Our commitment remains the same as it was on day one:
                        helping our customers succeed through quality products,
                        competitive pricing, prompt delivery and dependable
                        support.
                    </p>
                    <p class="about-commitment-quote">
                        "Personal direct service with great products —
                        that's who JPM Direct is and what we proudly offer."
                    </p>
                </div>
            </div>
        </section>
    </div>
</asp:Content>