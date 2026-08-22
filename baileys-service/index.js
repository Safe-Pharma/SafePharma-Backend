const { default: makeWASocket, useMultiFileAuthState, DisconnectReason } = require('@whiskeysockets/baileys');
const express = require('express');
const QRCode = require('qrcode');

const app = express();
app.use(express.json());

const PORT = process.env.PORT || 3001;
const INTERNAL_API_KEY = process.env.INTERNAL_API_KEY || 'dev-only-key-change-me';

let sock;
let latestQr = null;

async function startSock() {
    const { state, saveCreds } = await useMultiFileAuthState('auth_info');
    sock = makeWASocket({ auth: state });

    sock.ev.on('creds.update', saveCreds);

    sock.ev.on('connection.update', (update) => {
        const { connection, lastDisconnect, qr } = update;

        if (qr) {
            latestQr = qr;
            console.log('QR updated — visit /qr to scan it');
        }

        if (connection === 'close') {
            const shouldReconnect =
                lastDisconnect?.error?.output?.statusCode !== DisconnectReason.loggedOut;
            console.log('Connection closed, reconnecting:', shouldReconnect);
            if (shouldReconnect) startSock();
        } else if (connection === 'open') {
            latestQr = null;
            console.log('✅ WhatsApp connected successfully');
        }
    });
}

startSock();

app.get('/', (req, res) => {
    res.status(200).send('Baileys OTP service is running.');
});

app.get('/qr', async (req, res) => {
    if (!latestQr) {
        return res.send('<h2>No QR available — already connected, or not generated yet. Refresh in a few seconds.</h2>');
    }
    const qrImage = await QRCode.toDataURL(latestQr);
    res.send(`<img src="${qrImage}" />`);
});

app.post('/send-otp', async (req, res) => {
    const providedKey = req.headers['x-internal-key'];
    if (providedKey !== INTERNAL_API_KEY) {
        return res.status(401).json({ success: false, error: 'Unauthorized' });
    }

    const { phone, otp } = req.body;

    if (!phone || !otp) {
        return res.status(400).json({ success: false, error: 'phone and otp are required' });
    }

    try {
        const jid = `${phone.replace('+', '')}@s.whatsapp.net`;
        await sock.sendMessage(jid, {
            text: `Your SafePharma verification code is: ${otp}\nDo not share this code with anyone.`
        });
        res.json({ success: true });
    } catch (e) {
        console.error('Send failed:', e.message);
        res.status(500).json({ success: false, error: e.message });
    }
});

app.listen(PORT, () => console.log(`Baileys OTP service running on port ${PORT}`));