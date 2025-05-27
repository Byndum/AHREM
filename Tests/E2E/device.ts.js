import { Selector } from 'testcafe';

fixture`Device Management E2E`
    .page('http://localhost:5001'); // Update to match your frontend's local dev port

test('Add and remove a device', async t => {
    await t
        .typeText(Selector('input[placeholder="Name"]'), 'Test Device')
        .typeText(Selector('input[placeholder="Firmware"]'), '1.0.0')
        .typeText(Selector('input[placeholder="MAC"]'), 'DE:AD:BE:EF:00:01')
        .click(Selector('input[type="checkbox"]'))
        .click(Selector('button').withText('Add Device'));

    const deviceItem = Selector('li').withText('Test Device');
    await t.expect(deviceItem.exists).ok();

    await t.click(deviceItem.find('button').withText('Delete'));
    await t.expect(deviceItem.exists).notOk();
});
