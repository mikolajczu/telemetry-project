const startTime = Date.now();

window.addEventListener('beforeunload', async (e) => {
    e.preventDefault();
    const endTime = Date.now();
    const timeSpent = endTime - startTime;
    const tabTitle = document.title;

    const data = {tabTitle, timeSpent};

    try {
        await fetch('http://localhost:5196/TelemetrySessions/SendInformationToSession', {
            method: 'POST',
            body: JSON.stringify(data),
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: "include",
            //keepalive: true
        });
    } catch (err) {
        console.log(err)
    }
});