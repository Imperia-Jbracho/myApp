(() => {
    document.addEventListener('DOMContentLoaded', () => {
        const config = window.travelDashboardConfig ?? {};
        const baseApiUrl = typeof config.baseApiUrl === 'string' ? config.baseApiUrl : '/api/travel';

        const modal = document.getElementById('travel-modal');
        const form = document.getElementById('travel-form');
        const participantList = document.getElementById('participant-list');
        const errorSummary = document.getElementById('form-error-summary');
        const modalTitle = document.getElementById('modal-title');
        const modalSubtitle = document.getElementById('modal-subtitle');
        const travelIdField = document.getElementById('travel-id');
        const titleField = document.getElementById('travel-title');
        const destinationField = document.getElementById('travel-destination');
        const startField = document.getElementById('travel-start');
        const endField = document.getElementById('travel-end');
        const currencyField = document.getElementById('travel-currency');
        const budgetField = document.getElementById('travel-budget');
        const addParticipantButton = document.getElementById('add-participant');

        if (!modal || !form || !participantList) {
            return;
        }

        let participantData = [];
        let currentMode = 'create';

        const openButtons = document.querySelectorAll('[data-action="open-create"]');
        openButtons.forEach(button => {
            button.addEventListener('click', () => {
                openModal('create');
            });
        });

        const closeButtons = modal.querySelectorAll('[data-action="close-modal"]');
        closeButtons.forEach(button => {
            button.addEventListener('click', () => {
                closeModal();
            });
        });

        modal.addEventListener('click', event => {
            if (event.target === modal) {
                closeModal();
            }
        });

        addParticipantButton?.addEventListener('click', () => {
            participantData.push({ email: '', role: '' });
            renderParticipants();
        });

        form.addEventListener('submit', async event => {
            event.preventDefault();
            await submitForm();
        });

        const editButtons = document.querySelectorAll('[data-action="edit-travel"]');
        editButtons.forEach(button => {
            button.addEventListener('click', () => {
                const payloadText = button.getAttribute('data-travel');
                if (!payloadText) {
                    return;
                }

                try {
                    const payload = JSON.parse(payloadText);
                    openModal('edit', payload);
                } catch (error) {
                    console.error('No se pudo leer la información del viaje para editar.', error);
                }
            });
        });

        bindActionButtons('[data-action="duplicate-travel"]', async id => {
            await sendRequest(`${baseApiUrl}/${id}/duplicate`, 'POST');
        });

        bindActionButtons('[data-action="archive-travel"]', async id => {
            await sendRequest(`${baseApiUrl}/${id}/archive`, 'POST');
        });

        bindActionButtons('[data-action="unarchive-travel"]', async id => {
            await sendRequest(`${baseApiUrl}/${id}/unarchive`, 'POST');
        });

        bindActionButtons('[data-action="delete-travel"]', async id => {
            const confirmDelete = window.confirm('¿Seguro que deseas eliminar este viaje? Esta acción no se puede deshacer.');
            if (!confirmDelete) {
                return;
            }

            await sendRequest(`${baseApiUrl}/${id}`, 'DELETE');
        });

        function bindActionButtons(selector, handler) {
            const buttons = document.querySelectorAll(selector);
            buttons.forEach(button => {
                button.addEventListener('click', async () => {
                    const id = button.getAttribute('data-id');
                    if (!id) {
                        return;
                    }

                    button.setAttribute('disabled', 'disabled');
                    try {
                        await handler(id);
                    } catch (error) {
                        showToast('Ocurrió un problema. Intenta de nuevo.', 'error');
                        console.error(error);
                    } finally {
                        button.removeAttribute('disabled');
                    }
                });
            });
        }

        const summaryConfig = window.travelSummaryConfig ?? null;
        if (summaryConfig) {
            const travelIdValue = typeof summaryConfig.travelId === 'number' ? summaryConfig.travelId : Number.parseInt(summaryConfig.travelId, 10);
            if (!Number.isNaN(travelIdValue)) {
                const baseApiUrlValue = typeof summaryConfig.baseApiUrl === 'string' ? summaryConfig.baseApiUrl : '/api/travel';
                const sanitizedBaseUrl = baseApiUrlValue.endsWith('/') ? baseApiUrlValue.slice(0, -1) : baseApiUrlValue;
                const milestonesBaseUrl = `${sanitizedBaseUrl}/${travelIdValue}/milestones`;

                bindMilestoneForm('[data-summary-form="activity"]', 'activity', `${milestonesBaseUrl}/activities`, buildActivityPayload, 'Actividad añadida correctamente.');
                bindMilestoneForm('[data-summary-form="lodging"]', 'lodging', `${milestonesBaseUrl}/lodgings`, buildLodgingPayload, 'Alojamiento guardado correctamente.');
                bindMilestoneForm('[data-summary-form="restaurant"]', 'restaurant', `${milestonesBaseUrl}/restaurants`, buildRestaurantPayload, 'Reserva registrada correctamente.');
            }

            function bindMilestoneForm(selector, errorKey, endpoint, payloadBuilder, successMessage) {
                const form = document.querySelector(selector);
                if (!form) {
                    return;
                }

                const errorContainer = document.querySelector(`[data-summary-errors="${errorKey}"]`);

                form.addEventListener('submit', async event => {
                    event.preventDefault();
                    resetSummaryErrors(errorContainer);

                    const submitButton = form.querySelector('[type="submit"]');
                    if (submitButton) {
                        submitButton.setAttribute('disabled', 'disabled');
                    }

                    try {
                        const payload = payloadBuilder(form);
                        const response = await fetch(endpoint, {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify(payload)
                        });

                        if (response.ok) {
                            showToast(successMessage, 'success');
                            window.location.reload();
                            return;
                        }

                        const messages = await extractProblemMessages(response);
                        showSummaryErrors(errorContainer, messages);
                        showToast('No se pudo guardar la información.', 'error');
                    } catch (error) {
                        console.error(error);
                        showSummaryErrors(errorContainer, ['Ocurrió un error inesperado.']);
                        showToast('No se pudo guardar la información.', 'error');
                    } finally {
                        if (submitButton) {
                            submitButton.removeAttribute('disabled');
                        }
                    }
                });
            }

            function resetSummaryErrors(container) {
                if (!container) {
                    return;
                }

                container.classList.add('hidden');
                container.innerHTML = '';
            }

            function showSummaryErrors(container, messages) {
                if (!container) {
                    return;
                }

                const finalMessages = Array.isArray(messages) && messages.length > 0 ? messages : ['Revisa la información proporcionada.'];
                container.innerHTML = finalMessages.map(message => `<p>${message}</p>`).join('');
                container.classList.remove('hidden');
            }

            async function extractProblemMessages(response) {
                try {
                    const problem = await response.json();
                    if (problem && problem.errors) {
                        const messages = [];
                        Object.keys(problem.errors).forEach(key => {
                            const values = problem.errors[key];
                            if (Array.isArray(values)) {
                                values.forEach(message => messages.push(message));
                            }
                        });

                        if (messages.length > 0) {
                            return messages;
                        }
                    }

                    if (problem && typeof problem.detail === 'string' && problem.detail.length > 0) {
                        return [problem.detail];
                    }
                } catch (error) {
                    console.error('No se pudo leer la respuesta del servidor.', error);
                }

                return ['No se pudo guardar la información. Intenta nuevamente.'];
            }

            function buildActivityPayload(form) {
                const formData = new FormData(form);
                const costValue = readOptionalDecimal(formData, 'activity-price');

                return {
                    title: readFormDataString(formData, 'activity-title'),
                    date: readOptionalDate(formData, 'activity-date'),
                    classification: readOptionalString(formData, 'activity-classification'),
                    durationHours: readOptionalDecimal(formData, 'activity-duration'),
                    startTime: readOptionalTime(formData, 'activity-start'),
                    endTime: readOptionalTime(formData, 'activity-end'),
                    cost: costValue ?? 0,
                    locationUrl: readOptionalString(formData, 'activity-location')
                };
            }

            function buildLodgingPayload(form) {
                const formData = new FormData(form);
                const costValue = readOptionalDecimal(formData, 'lodging-price');

                return {
                    title: readFormDataString(formData, 'lodging-title'),
                    nights: readOptionalInteger(formData, 'lodging-nights'),
                    checkInDate: readOptionalDate(formData, 'lodging-checkin'),
                    checkOutDate: readOptionalDate(formData, 'lodging-checkout'),
                    cost: costValue ?? 0,
                    locationUrl: readOptionalString(formData, 'lodging-location'),
                    bookingPlatform: readOptionalString(formData, 'lodging-platform')
                };
            }

            function buildRestaurantPayload(form) {
                const formData = new FormData(form);
                const costValue = readOptionalDecimal(formData, 'restaurant-cost');

                return {
                    title: readFormDataString(formData, 'restaurant-name'),
                    reservationDate: readOptionalDate(formData, 'restaurant-date'),
                    startTime: readOptionalTime(formData, 'restaurant-time'),
                    cost: costValue ?? 0,
                    locationUrl: readOptionalString(formData, 'restaurant-location')
                };
            }

            function readFormDataString(formData, key) {
                const value = formData.get(key);
                return typeof value === 'string' ? value.trim() : '';
            }

            function readOptionalString(formData, key) {
                const value = readFormDataString(formData, key);
                return value.length > 0 ? value : null;
            }

            function readOptionalDecimal(formData, key) {
                const value = readFormDataString(formData, key);
                if (value.length === 0) {
                    return null;
                }

                const number = Number.parseFloat(value);
                return Number.isNaN(number) ? null : number;
            }

            function readOptionalInteger(formData, key) {
                const value = readFormDataString(formData, key);
                if (value.length === 0) {
                    return null;
                }

                const number = Number.parseInt(value, 10);
                return Number.isNaN(number) ? null : number;
            }

            function readOptionalDate(formData, key) {
                const value = readFormDataString(formData, key);
                return value.length > 0 ? value : null;
            }

            function readOptionalTime(formData, key) {
                const value = readFormDataString(formData, key);
                return value.length > 0 ? value : null;
            }
        }

        function openModal(mode, travel) {
            currentMode = mode;
            resetErrors();

            if (mode === 'edit' && travel) {
                modalTitle.textContent = 'Editar viaje';
                modalSubtitle.textContent = 'Actualiza la información de tu aventura';
                travelIdField.value = travel.id ?? '';
                titleField.value = travel.title ?? '';
                destinationField.value = travel.destination ?? '';
                startField.value = travel.startDate ?? '';
                endField.value = travel.endDate ?? '';
                currencyField.value = (travel.currency ?? '').toUpperCase();
                budgetField.value = travel.initialBudget != null ? travel.initialBudget : '';
                participantData = Array.isArray(travel.participants)
                    ? travel.participants.map(participant => ({
                        email: participant.email ?? '',
                        role: participant.role ?? ''
                    }))
                    : [];
            } else {
                modalTitle.textContent = 'Crear un viaje';
                modalSubtitle.textContent = 'Define la estructura básica de tu viaje';
                travelIdField.value = '';
                titleField.value = '';
                destinationField.value = '';
                startField.value = '';
                endField.value = '';
                currencyField.value = 'USD';
                budgetField.value = '';
                participantData = [{ email: '', role: '' }];
            }

            if (participantData.length === 0) {
                participantData = [{ email: '', role: '' }];
            }

            renderParticipants();

            modal.classList.remove('hidden');
            document.body.classList.add('overflow-hidden');
        }

        function closeModal() {
            modal.classList.add('hidden');
            document.body.classList.remove('overflow-hidden');
        }

        function renderParticipants() {
            participantList.innerHTML = '';

            participantData.forEach((participant, index) => {
                const wrapper = document.createElement('div');
                wrapper.className = 'grid gap-3 rounded-2xl border border-white/10 bg-slate-950/60 p-4 sm:grid-cols-12';

                const emailContainer = document.createElement('div');
                emailContainer.className = 'sm:col-span-5 flex flex-col gap-1 text-sm';
                const emailLabel = document.createElement('label');
                emailLabel.className = 'font-semibold text-white';
                emailLabel.textContent = 'Correo';
                const emailInput = document.createElement('input');
                emailInput.type = 'email';
                emailInput.name = `Participants[${index}].Email`;
                emailInput.required = false;
                emailInput.placeholder = 'persona@email.com';
                emailInput.value = participant.email ?? '';
                emailInput.className = 'rounded-2xl border border-white/10 bg-slate-900/80 px-4 py-2 text-sm text-white focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-500/40';
                emailInput.addEventListener('input', event => {
                    participantData[index].email = event.target.value;
                });
                emailContainer.appendChild(emailLabel);
                emailContainer.appendChild(emailInput);

                const roleContainer = document.createElement('div');
                roleContainer.className = 'sm:col-span-5 flex flex-col gap-1 text-sm';
                const roleLabel = document.createElement('label');
                roleLabel.className = 'font-semibold text-white';
                roleLabel.textContent = 'Rol';
                const roleInput = document.createElement('input');
                roleInput.type = 'text';
                roleInput.name = `Participants[${index}].Role`;
                roleInput.placeholder = 'Responsable, guía, etc.';
                roleInput.value = participant.role ?? '';
                roleInput.maxLength = 100;
                roleInput.className = 'rounded-2xl border border-white/10 bg-slate-900/80 px-4 py-2 text-sm text-white focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-500/40';
                roleInput.addEventListener('input', event => {
                    participantData[index].role = event.target.value;
                });
                roleContainer.appendChild(roleLabel);
                roleContainer.appendChild(roleInput);

                const actionContainer = document.createElement('div');
                actionContainer.className = 'sm:col-span-2 flex items-center justify-end';
                const removeButton = document.createElement('button');
                removeButton.type = 'button';
                removeButton.className = 'rounded-full border border-red-500/40 px-3 py-2 text-xs font-semibold text-red-200 transition hover:border-red-500 hover:text-red-100';
                removeButton.textContent = 'Eliminar';
                removeButton.addEventListener('click', () => {
                    if (participantData.length === 1) {
                        participantData[0] = { email: '', role: '' };
                    } else {
                        participantData.splice(index, 1);
                    }
                    renderParticipants();
                });
                actionContainer.appendChild(removeButton);

                wrapper.appendChild(emailContainer);
                wrapper.appendChild(roleContainer);
                wrapper.appendChild(actionContainer);

                participantList.appendChild(wrapper);
            });
        }

        function collectParticipants() {
            return participantData
                .map(participant => ({
                    email: typeof participant.email === 'string' ? participant.email.trim() : '',
                    role: typeof participant.role === 'string' ? participant.role.trim() : ''
                }))
                .filter(participant => participant.email.length > 0);
        }

        async function submitForm() {
            resetErrors();

            const payload = {
                title: titleField.value.trim(),
                destination: destinationField.value.trim(),
                startDate: startField.value,
                endDate: endField.value,
                currency: currencyField.value.trim().toUpperCase(),
                initialBudget: budgetField.value !== '' ? Number(budgetField.value) : null,
                participants: collectParticipants()
            };

            if (Number.isNaN(payload.initialBudget)) {
                payload.initialBudget = null;
            }

            let url = baseApiUrl;
            let method = 'POST';

            if (currentMode === 'edit') {
                const travelId = travelIdField.value;
                if (!travelId) {
                    showErrors(['No se encontró el identificador del viaje.']);
                    return;
                }

                url = `${baseApiUrl}/${travelId}`;
                method = 'PUT';
            }

            try {
                const response = await fetch(url, {
                    method,
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(payload)
                });

                if (response.ok) {
                    closeModal();
                    window.location.reload();
                    return;
                }

                const problem = await response.json().catch(() => null);
                if (problem && problem.errors) {
                    const messages = [];
                    Object.keys(problem.errors).forEach(key => {
                        const values = problem.errors[key];
                        if (Array.isArray(values)) {
                            values.forEach(message => messages.push(message));
                        }
                    });
                    showErrors(messages);
                } else {
                    showErrors(['No se pudo guardar el viaje. Intenta nuevamente.']);
                }
            } catch (error) {
                console.error(error);
                showErrors(['Ocurrió un error inesperado.']);
            }
        }

        async function sendRequest(url, method) {
            try {
                const response = await fetch(url, {
                    method,
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok) {
                    throw new Error(`Request failed with status ${response.status}`);
                }

                window.location.reload();
            } catch (error) {
                console.error(error);
                showToast('No se pudo completar la acción.', 'error');
            }
        }

        function resetErrors() {
            if (!errorSummary) {
                return;
            }

            errorSummary.classList.add('hidden');
            errorSummary.textContent = '';
        }

        function showErrors(messages) {
            if (!errorSummary) {
                return;
            }

            const finalMessages = Array.isArray(messages) && messages.length > 0 ? messages : ['Revisa la información del formulario.'];
            errorSummary.innerHTML = finalMessages.map(message => `<p>${message}</p>`).join('');
            errorSummary.classList.remove('hidden');
        }

        function showToast(message, type) {
            const container = document.createElement('div');
            container.className = `fixed left-1/2 top-6 z-[60] -translate-x-1/2 rounded-full px-5 py-3 text-sm font-semibold shadow-lg transition ${type === 'error' ? 'bg-red-500/90 text-white' : 'bg-brand-500 text-white'}`;
            container.textContent = message;
            document.body.appendChild(container);
            setTimeout(() => {
                container.classList.add('opacity-0');
                setTimeout(() => container.remove(), 300);
            }, 2500);
        }
    });
})();
