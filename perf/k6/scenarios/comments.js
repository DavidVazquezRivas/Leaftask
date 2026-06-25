import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL } from '../config.js';
import { headers } from '../setup.js';

export const commentsScenario = {
    readWrite(data) {
        const { token, projectId, workItemIds } = data;
        const itemId = workItemIds[__VU % workItemIds.length];

        // Listar comentarios
        const list = http.get(
            `${BASE_URL}/projects/${projectId}/work-items/${itemId}/comments`,
            { headers: headers(token) },
        );
        check(list, { 'list comments 200': (r) => r.status === 200 });

        // Añadir comentario
        const post = http.post(
            `${BASE_URL}/projects/${projectId}/work-items/${itemId}/comments`,
            JSON.stringify({ content: `k6 comment VU=${__VU} ITER=${__ITER}`, attachmentIds: [] }),
            { headers: headers(token) },
        );
        check(post, { 'post comment 201': (r) => r.status === 201 });

        sleep(1);
    },
};
