const startTime = Date.now();

window.addEventListener('beforeunload', () => {
    const endTime = Date.now();
    const timeSpent = endTime - startTime;
    const tabTitle = document.title;

    const data = { tabTitle, timeSpent };

    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/TelemetrySessions/SendInformationToSession');
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.send(JSON.stringify(data));
});