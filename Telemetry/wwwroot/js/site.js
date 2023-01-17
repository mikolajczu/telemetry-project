const startTime = Date.now();

window.addEventListener('beforeunload', () => {
    const endTime = Date.now();
    const timeSpent = endTime - startTime;
    const tabTitle = document.title;

    const data = { tabTitle, timeSpent };

    fetch('/TelemetrySessions/SendInformationToSession', {
        method: 'POST',
        body: JSON.stringify(data),
        headers: {
            'Content-Type': 'application/json'
        }
    });
});