<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="NUTRITRACK.Pages.Auth.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .nt-login-page {
            min-height: calc(100vh - 80px);
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 30px 15px;
            background:
                radial-gradient(circle at 20% 30%, rgba(255, 213, 90, .10), transparent 250px),
                radial-gradient(circle at 80% 60%, rgba(255, 213, 90, .08), transparent 280px),
                linear-gradient(180deg, #0b1220 0%, #121a2b 100%);
            overflow: hidden;
        }

        .nt-login-wrap {
            width: 100%;
            max-width: 1180px;
            display: grid;
            grid-template-columns: 300px 1fr;
            gap: 28px;
            align-items: center;
        }

        /* =========================
           LÁMPARA
        ========================= */
        .lamp-zone {
            position: relative;
            min-height: 540px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .lamp-glow {
            position: absolute;
            width: 380px;
            height: 380px;
            border-radius: 50%;
            background: radial-gradient(circle,
                rgba(255, 229, 134, .45) 0%,
                rgba(255, 229, 134, .22) 30%,
                rgba(255, 229, 134, .10) 55%,
                transparent 75%);
            filter: blur(8px);
            opacity: 0;
            transform: scale(.72);
            transition: all .45s ease;
            pointer-events: none;
        }

        .lamp-zone.on .lamp-glow {
            opacity: 1;
            transform: scale(1);
        }

        .lamp {
            position: relative;
            width: 190px;
            height: 330px;
            cursor: pointer;
            user-select: none;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: flex-start;
            transition: transform .25s ease;
        }

        .lamp:hover {
            transform: translateY(-4px);
        }

        .lamp-shade {
            width: 155px;
            height: 72px;
            background: #f8f4ea;
            border-radius: 90px 90px 26px 26px;
            box-shadow: 0 0 0 2px rgba(255,255,255,.08), 0 0 25px rgba(255,255,255,.05);
            position: relative;
            z-index: 2;
            transition: all .4s ease;
        }

        .lamp-pole {
            width: 18px;
            height: 145px;
            background: linear-gradient(180deg, #efe9de, #d8cfc2);
            margin-top: -2px;
            border-radius: 10px;
            position: relative;
            z-index: 1;
        }

        .lamp-base {
            width: 100px;
            height: 16px;
            background: linear-gradient(180deg, #ece6db, #cfc5b6);
            border-radius: 999px;
            margin-top: 7px;
        }

        .lamp-cord {
            position: absolute;
            right: 35px;
            top: 58px;
            width: 2px;
            height: 92px;
            background: rgba(255,255,255,.40);
        }

        .lamp-switch {
            position: absolute;
            right: 29px;
            top: 140px;
            width: 14px;
            height: 14px;
            border-radius: 50%;
            background: #d9a574;
            box-shadow: 0 0 0 2px rgba(0,0,0,.12);
            transition: transform .25s ease, box-shadow .25s ease;
        }

        .lamp-zone.on .lamp-shade {
            background: #fffaf0;
            box-shadow:
                0 0 18px rgba(255, 246, 203, .70),
                0 0 48px rgba(255, 223, 121, .42),
                0 0 95px rgba(255, 223, 121, .28);
        }

        .lamp-zone.on .lamp-switch {
            transform: translateY(8px);
            box-shadow: 0 0 15px rgba(255, 210, 110, .65);
        }

        /* =========================
           PANEL LOGIN
        ========================= */
        .login-panel {
            position: relative;
            border-radius: 30px;
            padding: 30px;
            background: rgba(255,255,255,.05);
            border: 1px solid rgba(255,255,255,.08);
            backdrop-filter: blur(8px);
            box-shadow: 0 18px 40px rgba(0,0,0,.22);
            overflow: hidden;
            transition: all .45s ease;
        }

        .login-panel::before {
            content: "";
            position: absolute;
            inset: -20%;
            background:
                radial-gradient(circle at 15% 30%, rgba(255, 221, 120, .22), transparent 20%),
                radial-gradient(circle at 45% 50%, rgba(255, 221, 120, .13), transparent 24%),
                radial-gradient(circle at 80% 55%, rgba(255, 221, 120, .08), transparent 24%);
            opacity: 0;
            transition: opacity .45s ease;
            pointer-events: none;
        }

        .login-panel.on {
            background: rgba(255,255,255,.08);
            border: 1px solid rgba(255, 221, 120, .18);
            box-shadow:
                0 20px 55px rgba(0,0,0,.30),
                0 0 60px rgba(255, 221, 120, .10);
        }

        .login-panel.on::before {
            opacity: 1;
        }

        .login-top {
            display: grid;
            grid-template-columns: 320px 1fr;
            gap: 26px;
            align-items: center;
        }

        /* =========================
           TRACKI
        ========================= */
        .trackie-side {
            min-height: 390px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .trackie-box {
            text-align: center;
            position: relative;
        }

        .trackie-frame {
            position: relative;
            width: 240px;
            min-height: 280px;
            margin: 0 auto;
        }

        .trackie-img {
            width: 230px;
            max-width: 100%;
            height: auto;
            display: block;
            margin: 0 auto;
            filter: drop-shadow(0 10px 18px rgba(0,0,0,.25));
            transition: opacity .35s ease, transform .35s ease;
        }

        .trackie-img.hidden {
            opacity: 0;
            transform: translateY(10px) scale(.96);
            position: absolute;
            left: 0;
            right: 0;
            margin: auto;
            pointer-events: none;
        }

        .trackie-img.active {
            opacity: 1;
            transform: translateY(0) scale(1);
            position: relative;
        }

        .speech-bubble {
            position: relative;
            display: inline-block;
            max-width: 250px;
            background: rgba(255,255,255,.96);
            color: #172030;
            border-radius: 18px;
            padding: 14px 18px;
            margin-bottom: 16px;
            font-weight: 700;
            line-height: 1.4;
            box-shadow: 0 10px 20px rgba(0,0,0,.16);
            transition: all .25s ease;
        }

        .speech-bubble::after {
            content: "";
            position: absolute;
            left: 40px;
            bottom: -12px;
            width: 0;
            height: 0;
            border-left: 12px solid transparent;
            border-right: 12px solid transparent;
            border-top: 12px solid rgba(255,255,255,.96);
        }

        .trackie-side.error-mode .speech-bubble {
            background: #fff2f4;
            color: #8a2030;
        }

        .trackie-side.error-mode .speech-bubble::after {
            border-top-color: #fff2f4;
        }

        .trackie-side.success-mode .speech-bubble {
            background: #f0fff6;
            color: #146c43;
        }

        .trackie-side.success-mode .speech-bubble::after {
            border-top-color: #f0fff6;
        }

        /* =========================
           FORM
        ========================= */
        .login-card {
            border-radius: 24px;
            padding: 30px;
            background: rgba(20, 24, 33, .62);
            border: 1px solid rgba(255,255,255,.08);
            box-shadow: inset 0 0 0 1px rgba(255,255,255,.03);
            transition: all .35s ease;
        }

        .login-panel:not(.on) .login-card {
            opacity: .58;
            transform: scale(.985);
            filter: grayscale(.15);
        }

        .login-title {
            color: #fff;
            font-size: 2rem;
            font-weight: 800;
            margin-bottom: 6px;
        }

        .login-subtitle {
            color: rgba(255,255,255,.72);
            margin-bottom: 22px;
        }

        .form-label {
            color: rgba(255,255,255,.88);
            font-weight: 600;
        }

        .nt-input {
            border-radius: 14px !important;
            min-height: 50px;
            background: rgba(255,255,255,.08) !important;
            border: 1px solid rgba(255,255,255,.12) !important;
            color: #fff !important;
            box-shadow: none !important;
        }

        .nt-input::placeholder {
            color: rgba(255,255,255,.45);
        }

        .nt-input:focus {
            border-color: rgba(255, 221, 120, .55) !important;
            box-shadow: 0 0 0 0.20rem rgba(255, 221, 120, .12) !important;
            background: rgba(255,255,255,.10) !important;
        }

        .btn-lamp {
            min-height: 50px;
            border: 0;
            border-radius: 14px;
            font-weight: 800;
            letter-spacing: .2px;
            color: #2d2410;
            background: linear-gradient(135deg, #f7e8a2 0%, #f2d978 35%, #fff1be 50%, #e1bc53 100%);
            box-shadow: 0 10px 20px rgba(229, 189, 84, .22);
            transition: transform .2s ease, box-shadow .2s ease;
        }

        .btn-lamp:hover {
            transform: translateY(-1px);
            box-shadow: 0 14px 28px rgba(229, 189, 84, .30);
        }

        .btn-lamp:disabled {
            opacity: .7;
            cursor: not-allowed;
        }

        .disabled-note {
            color: rgba(255,255,255,.60);
            font-size: .92rem;
            margin-top: 10px;
        }

        .login-state {
            margin-top: 16px;
            border-radius: 14px;
            padding: 12px 14px;
            font-weight: 700;
            display: none;
            animation: fadeInUp .35s ease;
        }

        .login-state.show {
            display: block;
        }

        .login-state.error {
            background: rgba(220, 53, 69, .13);
            color: #ffd4da;
            border: 1px solid rgba(220, 53, 69, .30);
        }

        .login-state.success {
            background: rgba(25, 135, 84, .14);
            color: #c8f4da;
            border: 1px solid rgba(25, 135, 84, .28);
        }

        .helper-row {
            display: flex;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
            margin-top: 16px;
        }

        .helper-link {
            color: #ffe08a;
            text-decoration: none;
            font-weight: 600;
        }

        .helper-link:hover {
            color: #fff0bb;
            text-decoration: underline;
        }

        .shake {
            animation: shakeCard .45s ease;
        }

        @keyframes fadeInUp {
            from { opacity: 0; transform: translateY(8px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        @keyframes shakeCard {
            0% { transform: translateX(0); }
            20% { transform: translateX(-6px); }
            40% { transform: translateX(6px); }
            60% { transform: translateX(-4px); }
            80% { transform: translateX(4px); }
            100% { transform: translateX(0); }
        }

        @media (max-width: 991px) {
            .nt-login-wrap {
                grid-template-columns: 1fr;
            }

            .lamp-zone {
                min-height: 240px;
            }

            .login-top {
                grid-template-columns: 1fr;
            }

            .trackie-side {
                min-height: auto;
            }

            .trackie-frame {
                width: 210px;
                min-height: 250px;
            }

            .trackie-img {
                width: 200px;
            }
        }
    </style>

    <div class="nt-login-page">
        <div class="nt-login-wrap">

            <!-- LAMP -->
            <div id="lampZone" class="lamp-zone">
                <div class="lamp-glow"></div>

                <div class="lamp" onclick="toggleLampLogin()" title="Activar login">
                    <div class="lamp-shade"></div>
                    <div class="lamp-pole"></div>
                    <div class="lamp-base"></div>
                    <div class="lamp-cord"></div>
                    <div class="lamp-switch"></div>
                </div>
            </div>

            <!-- LOGIN PANEL -->
            <div id="loginPanel" class="login-panel">
                <div class="login-top">

                    <!-- TRACKIE -->
                    <div id="trackieSide" class="trackie-side">
                        <div class="trackie-box">

                            <div id="trackieBubble" class="speech-bubble">
                                Da clic en la lámpara para activar tu acceso
                            </div>

                            <div class="trackie-frame">
                                <img id="trackieNormal"
                                     src="<%= ResolveUrl("~/Image/Saludo.png") %>"
                                     alt="Trackie saludo"
                                     class="trackie-img active" />

                                <img id="trackieError"
                                     src="<%= ResolveUrl("~/Image/TrackiError.png") %>"
                                     alt="Trackie error"
                                     class="trackie-img hidden" />
                            </div>
                        </div>
                    </div>

                    <!-- FORM -->
                    <div id="loginCard" class="login-card">
                        <div class="login-title">Iniciar sesión</div>
                        <div class="login-subtitle">Activa el panel y entra a tu cuenta de NutriTrack</div>

                        <asp:Label ID="lblMsg" runat="server" CssClass="d-none"></asp:Label>

                        <div class="mb-3">
                            <label class="form-label">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control nt-input" TextMode="Email" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Contraseña</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control nt-input" TextMode="Password" />
                        </div>

                        <asp:Button ID="btnLogin" runat="server"
                            CssClass="btn btn-lamp w-100"
                            Text="Entrar"
                            OnClientClick="return beforeSubmitLogin();"
                            OnClick="btnLogin_Click" />

                        <div id="loginDisabledNote" class="disabled-note">
                            El formulario está apagado.
                        </div>

                        <div id="loginStateBox" class="login-state"></div>

                        <div class="helper-row">
                            <a class="helper-link" href="<%= ResolveUrl("~/Pages/Auth/Register.aspx") %>">Crear cuenta</a>
                        </div>
                    </div>

                </div>
            </div>

        </div>
    </div>

    <script>
        let lampOn = false;

        function getLoginElements() {
            return {
                lampZone: document.getElementById('lampZone'),
                loginPanel: document.getElementById('loginPanel'),
                loginCard: document.getElementById('loginCard'),
                trackieSide: document.getElementById('trackieSide'),
                trackieBubble: document.getElementById('trackieBubble'),
                trackieNormal: document.getElementById('trackieNormal'),
                trackieError: document.getElementById('trackieError'),
                stateBox: document.getElementById('loginStateBox'),
                disabledNote: document.getElementById('loginDisabledNote'),
                txtEmail: document.getElementById('<%= txtEmail.ClientID %>'),
                txtPassword: document.getElementById('<%= txtPassword.ClientID %>'),
                btnLogin: document.getElementById('<%= btnLogin.ClientID %>'),
                lblMsg: document.getElementById('<%= lblMsg.ClientID %>')
            };
        }

        function showTrackie(mode) {
            const el = getLoginElements();

            el.trackieNormal.classList.remove('active');
            el.trackieNormal.classList.add('hidden');

            el.trackieError.classList.remove('active');
            el.trackieError.classList.add('hidden');

            el.trackieSide.classList.remove('error-mode');
            el.trackieSide.classList.remove('success-mode');

            if (mode === 'error') {
                el.trackieError.classList.remove('hidden');
                el.trackieError.classList.add('active');
                el.trackieSide.classList.add('error-mode');
            } else {
                el.trackieNormal.classList.remove('hidden');
                el.trackieNormal.classList.add('active');

                if (mode === 'success') {
                    el.trackieSide.classList.add('success-mode');
                }
            }
        }

        function setFormEnabled(enabled) {
            const el = getLoginElements();

            if (el.txtEmail) el.txtEmail.disabled = !enabled;
            if (el.txtPassword) el.txtPassword.disabled = !enabled;
            if (el.btnLogin) el.btnLogin.disabled = !enabled;

            if (enabled) {
                el.lampZone.classList.add('on');
                el.loginPanel.classList.add('on');
                el.disabledNote.innerText = 'Panel activado. Ya puedes iniciar sesión.';
                el.trackieBubble.innerText = '¡Bienvenido a NutriTrack!';
                showTrackie('normal');
            } else {
                el.lampZone.classList.remove('on');
                el.loginPanel.classList.remove('on');
                el.disabledNote.innerText = 'El formulario está apagado.';
                el.trackieBubble.innerText = 'Da clic en la lámpara para activar tu acceso';
                showTrackie('normal');
                hideState();
            }
        }

        function toggleLampLogin() {
            lampOn = !lampOn;
            setFormEnabled(lampOn);
        }

        function showState(message, type) {
            const el = getLoginElements();
            el.stateBox.className = 'login-state show ' + type;
            el.stateBox.innerText = message;
        }

        function hideState() {
            const el = getLoginElements();
            el.stateBox.className = 'login-state';
            el.stateBox.innerText = '';
        }

        function shakeLoginCard() {
            const el = getLoginElements();
            el.loginCard.classList.remove('shake');
            void el.loginCard.offsetWidth;
            el.loginCard.classList.add('shake');
        }

        function beforeSubmitLogin() {
            const el = getLoginElements();

            if (!lampOn) {
                showState('Primero activa la lámpara para encender el login.', 'error');
                el.trackieBubble.innerText = 'Ey, primero enciende la lámpara ✨';
                showTrackie('error');
                shakeLoginCard();
                return false;
            }

            const email = (el.txtEmail.value || '').trim();
            const password = (el.txtPassword.value || '').trim();

            if (email === '' || password === '') {
                showState('Completa email y contraseña.', 'error');
                el.trackieBubble.innerText = 'Te faltan datos para entrar.';
                showTrackie('error');
                shakeLoginCard();
                return false;
            }

            hideState();
            el.trackieBubble.innerText = 'Validando acceso...';
            showTrackie('normal');
            return true;
        }

        function applyServerMessage() {
            const el = getLoginElements();
            if (!el.lblMsg) return;

            const serverMsg = (el.lblMsg.innerText || el.lblMsg.textContent || '').trim();

            if (serverMsg !== '') {
                lampOn = true;
                setFormEnabled(true);
                showState(serverMsg, 'error');
                el.trackieBubble.innerText = 'Ups... revisa tus datos e inténtalo de nuevo.';
                showTrackie('error');
                shakeLoginCard();
            }
        }

        window.addEventListener('load', function () {
            setFormEnabled(false);
            applyServerMessage();
        });
    </script>

</asp:Content>