import { Selector } from 'testcafe';

fixture`Device Management E2E`
    .page('https://localhost:7089'); // Frontend url

test('Add and remove a device', async t => {
    await t
        .typeText(Selector('input[placeholder="Firmware"]'), '1.50.1')
        .typeText(Selector('input[placeholder="MAC"]'), 'DE:AD:BE:EF:00:01')
        .click(Selector('input[type="checkbox"]'))
        .click(Selector('button').withText('Add Device'));

    const deviceItem = Selector('li').withText('1.50.1').withText('DE:AD:BE:EF:00:01');
    await t.expect(deviceItem.exists).ok();

    await t.click(deviceItem.find('button').withText('Delete'));
    await t.expect(deviceItem.exists).notOk();
});