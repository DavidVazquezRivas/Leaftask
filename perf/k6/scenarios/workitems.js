import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL, STATUS_ID, TYPE_ID } from '../config.js';
import { headers } from '../setup.js';

export const workitemsScenario = {
    read(data) {
        const { token, projectId, workItemIds } = data;
        const itemId = workItemIds[__VU % workItemIds.length];

        // Lista de workitems del proyecto
        const list = http.get(`${BASE_URL}/workitems/${projectId}`, { headers: headers(token) });
        check(list, { 'list workitems 200': (r) => r.status === 200 });

        // Detalle de un workitem específico
        const detail = http.get(`${BASE_URL}/workitems/${projectId}/${itemId}`, { headers: headers(token) });
        check(detail, { 'get workitem 200': (r) => r.status === 200 });

        sleep(0.5);
    },

    write(data) {
        const { token, projectId, workItemIds } = data;
        const itemId = workItemIds[__VU % workItemIds.length];

        // Actualizar un workitem existente
        const patch = http.patch(
            `${BASE_URL}/workitems/${projectId}/${itemId}`,
            JSON.stringify({ title: `Updated by VU ${__VU} iter ${__ITER}` }),
            { headers: headers(token) },
        );
        check(patch, { 'patch workitem 200': (r) => r.status === 200 });

        // Añadir un work log
        const log = http.post(
            `${BASE_URL}/workitems/${projectId}/${itemId}/work-logs`,
            JSON.stringify({ dedication: 0.5, date: new Date().toISOString().slice(0, 10), description: 'k6 log' }),
            { headers: headers(token) },
        );
        check(log, { 'create worklog 201': (r) => r.status === 201 });

        sleep(1);
    },
};
