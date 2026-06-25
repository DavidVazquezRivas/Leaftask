import http from 'k6/http';
import { check, fail } from 'k6';
import { BASE_URL, STATUS_ID, TYPE_ID } from './config.js';

function headers(token) {
    return { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' };
}

function devLogin(email) {
    const res = http.post(`${BASE_URL}/session/dev-login?email=${encodeURIComponent(email)}`);
    check(res, { 'dev-login 200': (r) => r.status === 200 }) ||
        fail(`dev-login failed for ${email}: ${res.status} ${res.body}`);
    return res.json('data.accessToken');
}

function createOrg(token) {
    const res = http.post(
        `${BASE_URL}/organizations`,
        JSON.stringify({ name: 'Perf Test Org', description: 'k6 load test', website: 'https://perf.test' }),
        { headers: headers(token) },
    );
    check(res, { 'create org 201': (r) => r.status === 201 }) ||
        fail(`create org failed: ${res.status} ${res.body}`);
    return res.json('data.id');
}

function createProject(token, organizationId) {
    const res = http.post(
        `${BASE_URL}/projects`,
        JSON.stringify({
            name: 'Perf Project',
            abbreviation: 'PRF',
            privacyLevel: 'Public',
            organizationId,
        }),
        { headers: headers(token) },
    );
    check(res, { 'create project 201': (r) => r.status === 201 }) ||
        fail(`create project failed: ${res.status} ${res.body}`);
    return res.json('data.id');
}

function createWorkItem(token, projectId, index) {
    const res = http.post(
        `${BASE_URL}/workitems/${projectId}`,
        JSON.stringify({
            title: `Perf WorkItem ${index}`,
            description: 'Created by k6 setup',
            estimation: 1,
            typeId: TYPE_ID,
            statusId: STATUS_ID,
            parentId: projectId,
            customFields: {},
        }),
        { headers: headers(token) },
    );
    check(res, { 'create workitem 201': (r) => r.status === 201 }) ||
        fail(`create workitem ${index} failed: ${res.status} ${res.body}`);
    return res.json('data.id');
}

export function setup() {
    const token = devLogin('admin@dev.com');
    const orgId = createOrg(token);
    const projectId = createProject(token, orgId);

    const workItemIds = [];
    for (let i = 0; i < 20; i++) {
        workItemIds.push(createWorkItem(token, projectId, i));
    }

    return { token, orgId, projectId, workItemIds };
}

export function teardown(data) {
    // Eliminar la organización de prueba (en cascada elimina proyecto y workitems)
    http.del(
        `${BASE_URL}/organizations/${data.orgId}`,
        null,
        { headers: headers(data.token) },
    );
}

export { headers };
