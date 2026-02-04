const puppeteer = require('puppeteer');

(async () => {
    console.log('Starting browser verification...');
    const browser = await puppeteer.launch({ headless: true });
    const page = await browser.newPage();

    // Capture logs
    page.on('console', msg => console.log('PAGE LOG:', msg.text()));
    page.on('pageerror', err => console.log('PAGE ERROR:', err.toString()));
    page.on('request', request => console.log(`REQ: ${request.url()}`));
    page.on('response', response => console.log(`RESP: ${response.status()} ${response.url()}`));
    page.on('requestfailed', request => console.log(`REQ FAIL: ${request.failure().errorText} ${request.url()}`));

    try {
        // 1. Goto Signin Page
        console.log('Navigating to http://localhost:4200/auth/signin...');
        const navigationPromise = page.goto('http://localhost:4200/auth/signin', { waitUntil: 'domcontentloaded' });
        const mainJsPromise = page.waitForResponse(res => res.url().includes('main.js') && res.status() === 200, { timeout: 10000 });

        await Promise.all([navigationPromise, mainJsPromise]);
        console.log('main.js loaded successfully.');

        // 2. Wait for content
        try {
            await page.waitForSelector('mat-card-title', { timeout: 5000 });
        } catch (e) {
            console.log("Timeout waiting for mat-card-title. Dumping HTML:");
            console.log(await page.content());
        }

        const title = await page.title();
        console.log(`Page Title: ${title}`);

        // Check text again
        const signinText = await page.evaluate(() => document.body.innerText.includes('Sign In'));
        if (signinText) {
            console.log('SUCCESS: "Sign In" text found on page.');
        } else {
            console.error('FAILURE: "Sign In" text NOT found.');
            process.exit(1);
        }

        // 3. Check Google Button container
        const googleBtn = await page.$('#google-btn');
        if (googleBtn) {
            console.log('SUCCESS: Google Button container found.');
        } else {
            console.error('FAILURE: Google Button container NOT found.');
        }

        // 4. Test Navigation to Signup
        console.log('Clicking "Sign Up" link...');
        try {
            // Wait for the link to be available
            await page.waitForSelector('a[href="/auth/signup"]', { timeout: 5000 });
            await Promise.all([
                page.click('a[href="/auth/signup"]'),
                page.waitForNavigation({ waitUntil: 'networkidle0' })
            ]);
            console.log('Navigated to Signup page.');
        } catch (e) {
            console.log('Using fallback text search for Sign Up link...');
            const links = await page.$$('a');
            let signupLink;
            for (const link of links) {
                const text = await page.evaluate(el => el.textContent, link);
                if (text.includes('Sign Up')) {
                    signupLink = link;
                    break;
                }
            }
            if (signupLink) {
                await Promise.all([
                    signupLink.click(),
                    page.waitForNavigation({ waitUntil: 'networkidle0' })
                ]);
                console.log('Navigated to Signup page via text match.');
            } else {
                throw new Error('Sign Up link not found');
            }
        }

        const url = page.url();
        console.log(`Current URL: ${url}`);
        if (!url.includes('/auth/signup')) {
            throw new Error(`Failed to navigate to signup. Current URL: ${url}`);
        }

        // 5. Fill Signup Form
        const uniqueId = Date.now();
        const email = `puppeteer_${uniqueId}@test.com`;
        const username = `user_${uniqueId}`;

        console.log(`Filling Signup Form with ${email}...`);
        await page.waitForSelector('input[formControlName="username"]');
        await page.type('input[formControlName="username"]', username);
        await page.type('input[formControlName="email"]', email);
        await page.type('input[formControlName="password"]', 'Password123!');

        // Submit
        console.log('Submitting Signup Form...');
        await page.click('button[type="submit"]');

        // Wait for redirect to Signin
        await page.waitForNavigation({ waitUntil: 'networkidle0' });
        console.log(`URL after signup: ${page.url()}`);

        if (!page.url().includes('/auth/signin')) {
            throw new Error('Did not redirect to signin page after signup');
        }

        // 6. Sign In
        console.log('Signing In...');
        await page.waitForSelector('input[formControlName="email"]');
        await page.type('input[formControlName="email"]', email);
        await page.type('input[formControlName="password"]', 'Password123!');

        await page.click('button[type="submit"]');

        // Wait for Dashboard
        await page.waitForNavigation({ waitUntil: 'networkidle0' });
        console.log(`URL after login: ${page.url()}`);

        if (page.url().includes('/dashboard')) {
            console.log('SUCCESS: Accessed Dashboard.');
        } else {
            throw new Error('Failed to access Dashboard');
        }

    } catch (error) {
        console.error('Error during verification:', error);
        process.exit(1);
    } finally {
        await browser.close();
    }
})();
