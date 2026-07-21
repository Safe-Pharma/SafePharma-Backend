const { default: makeWASocket, useMultiFileAuthState, DisconnectReason } = require('@whiskeysockets/baileys');
const express = require('express');
const qrcode = require('qrcode-terminal');

const app = express();
app.use(express.json());

let sock;

async function startSock() {
    const { state, saveCreds } = await useMultiFileAuthState('auth_info');
    sock = makeWASocket({ auth: state });

    sock.ev.on('creds.update', saveCreds);

    sock.ev.on('connection.update', (update) => {
        const { connection, lastDisconnect, qr } = update;

        if (qr) {
            console.log('Scan this QR code with WhatsApp:');
            qrcode.generate(qr, { small: true });
        }

        if (connection === 'close') {
            const shouldReconnect =
                lastDisconnect?.error?.output?.statusCode !== DisconnectReason.loggedOut;
            console.log('Connection closed, reconnecting:', shouldReconnect);
            if (shouldReconnect) startSock();
        } else if (connection === 'open') {
            console.log('✅ WhatsApp connected successfully');
        }
    });
}

startSock();

app.post('/send-otp', async (req, res) => {
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

app.listen(3001, () => console.log('Baileys OTP service running on http://localhost:3001'));